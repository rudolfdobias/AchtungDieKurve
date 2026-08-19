using System;
using System.Collections.Generic;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.AI
{
    /// <summary>
    /// Feeler-based steering: rays around the heading measure free distance;
    /// the kurve steers into open space and occasionally cuts off an enemy.
    /// Aggressiveness and Precision come from Properties, jittered per player.
    /// </summary>
    public class AiDriver
    {
        private static readonly float[] FeelerOffsets =
        {
            0f,
            MathHelper.ToRadians(25), -MathHelper.ToRadians(25),
            MathHelper.ToRadians(55), -MathHelper.ToRadians(55),
            MathHelper.ToRadians(90), -MathHelper.ToRadians(90),
        };

        private const float StraightBias = 1.15f;
        private const float AttackVetoFactor = 0.35f;
        private const int InterceptLeadFrames = 40;
        private const int AttackDurationMs = 2000;

        private readonly GridRegister _register;
        private readonly IContainer _container;
        private readonly PlayersManager _players;

        private struct Circle
        {
            public Vector2 Center;
            public float Radius;
        }

        public AiDriver(IContainer container, PlayersManager players, ref GridRegister register)
        {
            _container = container;
            _players = players;
            _register = register;
        }

        public void ControlAi(Kurve player, KeyboardState keyboardState, GameTime gameTime)
        {
            var ai = player as AiPlayer;
            if (ai == null || !ai.IsAlive) { return; }

            if (ai.DecisionCooldown-- <= 0)
            {
                Decide(ai, gameTime);
                ai.DecisionCooldown = 1 + (int)Math.Round((1 - ai.Precision) * 9);
            }

            if (ai.SteerCommand > 0) { ai.TurnRight(); }
            else if (ai.SteerCommand < 0) { ai.TurnLeft(); }
        }

        private void Decide(AiPlayer ai, GameTime gameTime)
        {
            var lookahead = Lookahead(ai);
            var obstacles = SenseObstacles(ai, gameTime, lookahead);

            var free = new float[FeelerOffsets.Length];
            for (var i = 0; i < FeelerOffsets.Length; i++)
            {
                free[i] = FreeDistance(ai, ai.Angle + FeelerOffsets[i], lookahead, obstacles);
            }

            var desired = SurviveHeading(ai, free);

            UpdateAttackState(ai, gameTime);
            if (ai.AttackTarget != null)
            {
                var attackHeading = InterceptHeading(ai);
                if (FreeDistance(ai, attackHeading, lookahead, obstacles) >= lookahead * AttackVetoFactor)
                {
                    desired = attackHeading;
                }
            }

            var sloppiness = 1 - ai.Precision;
            if (sloppiness > 0 && GameBase.Defaults.Rand.NextDouble() < sloppiness * 0.3)
            {
                desired += (float)(GameBase.Defaults.Rand.NextDouble() * 2 - 1) * MathHelper.ToRadians(30) * sloppiness;
            }

            var diff = MathHelper.WrapAngle(desired - ai.Angle);
            ai.SteerCommand = Math.Abs(diff) < ai.TurnStep ? 0 : Math.Sign(diff);
        }

        private static float Lookahead(Kurve player)
        {
            return Math.Max(120, 24 * player.Diameter);
        }

        private float SurviveHeading(AiPlayer ai, float[] free)
        {
            var bestIndex = 0;
            var bestScore = free[0] * StraightBias;
            for (var i = 1; i < free.Length; i++)
            {
                if (free[i] > bestScore)
                {
                    bestScore = free[i];
                    bestIndex = i;
                }
            }
            return ai.Angle + FeelerOffsets[bestIndex];
        }

        // Walls are measured analytically against the playfield edges and
        // powerups are harmless, so only kurve trails become obstacle circles.
        private List<Circle> SenseObstacles(AiPlayer ai, GameTime gameTime, float lookahead)
        {
            var result = new List<Circle>();
            foreach (var company in _register.Neighborhood(ai, gameTime, ai.CollisionCondition, (int)lookahead))
            {
                if (!(company.Owner is Kurve)) { continue; }
                result.Add(new Circle { Center = company.Center, Radius = company.Bounds.Width / 2f });
            }
            return result;
        }

        private float FreeDistance(AiPlayer ai, float heading, float lookahead, List<Circle> obstacles)
        {
            var origin = ai.AbsolutePosition;
            var direction = new Vector2((float)Math.Cos(heading), (float)Math.Sin(heading));
            var radius = ai.Diameter / 2f;

            var best = Math.Min(lookahead, BoundaryDistance(origin, direction, radius));
            foreach (var circle in obstacles)
            {
                var t = RayCircle(origin, direction, circle.Center, radius + circle.Radius);
                if (t >= 0 && t < best) { best = t; }
            }
            return Math.Max(best, 0);
        }

        private float BoundaryDistance(Vector2 origin, Vector2 direction, float radius)
        {
            var best = float.MaxValue;
            if (direction.X < 0) { best = Math.Min(best, (origin.X - radius) / -direction.X); }
            if (direction.X > 0) { best = Math.Min(best, (_container.ContentArea.Width - radius - origin.X) / direction.X); }
            if (direction.Y < 0) { best = Math.Min(best, (origin.Y - radius) / -direction.Y); }
            if (direction.Y > 0) { best = Math.Min(best, (_container.ContentArea.Height - radius - origin.Y) / direction.Y); }
            return best;
        }

        // First hit of a unit-direction ray against a circle; negative = behind or miss.
        private static float RayCircle(Vector2 origin, Vector2 direction, Vector2 center, float reach)
        {
            var offset = origin - center;
            var b = 2f * Vector2.Dot(offset, direction);
            var c = offset.LengthSquared() - reach * reach;
            var discriminant = b * b - 4f * c;
            if (discriminant < 0) { return -1; }
            return (-b - (float)Math.Sqrt(discriminant)) / 2f;
        }

        private void UpdateAttackState(AiPlayer ai, GameTime gameTime)
        {
            var now = gameTime.TotalGameTime.TotalMilliseconds;

            if (ai.AttackTarget != null && (now > ai.AttackUntil || !ai.AttackTarget.IsAlive))
            {
                ai.AttackTarget = null;
            }

            if (ai.NextAttackRoll <= 0)
            {
                ai.NextAttackRoll = now + AttackRollDelay();
                return;
            }
            if (ai.AttackTarget != null || now < ai.NextAttackRoll) { return; }

            ai.NextAttackRoll = now + AttackRollDelay();
            if (GameBase.Defaults.Rand.NextDouble() >= ai.Aggressiveness) { return; }

            ai.AttackTarget = NearestEnemy(ai);
            ai.AttackUntil = now + AttackDurationMs;
        }

        private static double AttackRollDelay()
        {
            return 3000 + GameBase.Defaults.Rand.NextDouble() * 5000;
        }

        private Kurve NearestEnemy(AiPlayer ai)
        {
            Kurve best = null;
            var bestDistance = float.MaxValue;
            foreach (var worm in _players.Worms)
            {
                if (worm == ai || !worm.IsAlive) { continue; }
                var distance = Vector2.DistanceSquared(worm.AbsolutePosition, ai.AbsolutePosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = worm;
                }
            }
            return best;
        }

        // Aim ahead of the target's nose to cut it off.
        private float InterceptHeading(AiPlayer ai)
        {
            var target = ai.AttackTarget;
            var lead = new Vector2((float)Math.Cos(target.Angle), (float)Math.Sin(target.Angle))
                       * target.Speed * InterceptLeadFrames;
            var aim = target.AbsolutePosition + lead - ai.AbsolutePosition;
            return (float)Math.Atan2(aim.Y, aim.X);
        }

        public void DrawDebug(SpriteBatch sb, AiPlayer player, GameTime gameTime)
        {
            if (player == null || !player.IsAlive || !GameBase.Defaults.DebugCollisions) { return; }

            var lookahead = Lookahead(player);
            var obstacles = SenseObstacles(player, gameTime, lookahead);
            foreach (var offset in FeelerOffsets)
            {
                var heading = player.Angle + offset;
                var distance = FreeDistance(player, heading, lookahead, obstacles);
                DrawRay(sb, player.AbsolutePosition, heading, distance,
                    distance < lookahead ? Color.OrangeRed : Color.LimeGreen);
            }
        }

        private static void DrawRay(SpriteBatch sb, Vector2 from, float heading, float length, Color color)
        {
            sb.Draw(CommonResources.whitepixel,
                new Rectangle((int)from.X, (int)from.Y, (int)length, 1),
                null, color, heading, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
