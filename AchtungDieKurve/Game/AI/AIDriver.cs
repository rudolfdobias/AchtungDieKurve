using System;
using System.Collections.Generic;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using AchtungDieKurve.Game.Drawable.Powerups;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.AI
{
    /// <summary>
    /// Feeler-based steering: rays around the heading measure free distance;
    /// the kurve steers into open space, occasionally cuts off an enemy and
    /// seeks useful powerups (avoiding Death). Aggressiveness and Precision
    /// come from Properties, jittered per player.
    /// </summary>
    public class AiDriver
    {
        // 15-degree resolution so narrow gaps in trails stay visible.
        private static readonly float[] FeelerOffsets = CreateFeelerOffsets(6, MathHelper.ToRadians(15));

        private const float StraightBias = 1.15f;
        private const float GoalVetoFactor = 0.35f;
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

        private static float[] CreateFeelerOffsets(int perSide, float step)
        {
            var offsets = new float[perSide * 2 + 1];
            offsets[0] = 0f;
            for (var i = 1; i <= perSide; i++)
            {
                offsets[i * 2 - 1] = step * i;
                offsets[i * 2] = -step * i;
            }
            return offsets;
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
            List<Circle> obstacles;
            List<Powerup> powerups;
            Sense(ai, gameTime, lookahead, out obstacles, out powerups);

            var free = new float[FeelerOffsets.Length];
            for (var i = 0; i < FeelerOffsets.Length; i++)
            {
                free[i] = FreeDistance(ai, ai.Angle + FeelerOffsets[i], lookahead, obstacles);
            }

            var desired = SurviveHeading(ai, free);

            UpdateAttackState(ai, gameTime);
            var goal = GoalHeading(ai, powerups);
            if (goal.HasValue && FreeDistance(ai, goal.Value, lookahead, obstacles) >= lookahead * GoalVetoFactor)
            {
                desired = goal.Value;
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

        /// <summary>
        /// Attack intercept or a desirable powerup, null when neither applies.
        /// A powerup wins over the intercept when it is much closer.
        /// </summary>
        private float? GoalHeading(AiPlayer ai, List<Powerup> powerups)
        {
            var attacking = ai.AttackTarget != null;
            var pickup = NearestDesirable(ai, powerups, attacking);

            if (attacking)
            {
                if (pickup != null)
                {
                    var pickupDistance = Vector2.DistanceSquared(PowerupCenter(pickup), ai.AbsolutePosition);
                    var targetDistance = Vector2.DistanceSquared(ai.AttackTarget.AbsolutePosition, ai.AbsolutePosition);
                    if (pickupDistance < targetDistance * 0.25f)
                    {
                        return HeadingTo(ai, PowerupCenter(pickup));
                    }
                }
                return InterceptHeading(ai);
            }

            return pickup != null ? HeadingTo(ai, PowerupCenter(pickup)) : (float?)null;
        }

        private static bool IsDesirable(Powerup powerup, bool attacking)
        {
            if (powerup is Switch) { return true; }
            if (attacking)
            {
                return powerup is Fast || powerup is SlowEnemy || powerup is FatEnemy;
            }
            return powerup is Slow || powerup is Slim || powerup is Transcend;
        }

        private Powerup NearestDesirable(AiPlayer ai, List<Powerup> powerups, bool attacking)
        {
            Powerup best = null;
            var bestDistance = float.MaxValue;
            foreach (var powerup in powerups)
            {
                if (!IsDesirable(powerup, attacking)) { continue; }
                var distance = Vector2.DistanceSquared(PowerupCenter(powerup), ai.AbsolutePosition);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = powerup;
                }
            }
            return best;
        }

        private static Vector2 PowerupCenter(Powerup powerup)
        {
            return powerup.Postition + new Vector2(Powerup.Width / 2f, Powerup.Height / 2f);
        }

        private static float HeadingTo(AiPlayer ai, Vector2 point)
        {
            var direction = point - ai.AbsolutePosition;
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        // Kurve trails become obstacle circles; Death powerups are obstacles
        // too so the feelers route around them. Other collectable powerups are
        // returned for goal seeking. Walls are measured analytically.
        private void Sense(AiPlayer ai, GameTime gameTime, float lookahead,
            out List<Circle> obstacles, out List<Powerup> powerups)
        {
            obstacles = new List<Circle>();
            powerups = new List<Powerup>();

            foreach (var company in _register.Neighborhood(ai, gameTime, ai.CollisionCondition, (int)lookahead))
            {
                if (company.Owner is Kurve)
                {
                    obstacles.Add(new Circle { Center = company.Center, Radius = company.Bounds.Width / 2f });
                    continue;
                }

                var powerup = company.Owner as Powerup;
                if (powerup == null || !powerup.CanBeHit) { continue; }

                if (powerup is Death)
                {
                    obstacles.Add(new Circle { Center = PowerupCenter(powerup), Radius = Powerup.Width / 2f });
                }
                else
                {
                    powerups.Add(powerup);
                }
            }
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
            List<Circle> obstacles;
            List<Powerup> powerups;
            Sense(player, gameTime, lookahead, out obstacles, out powerups);

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
