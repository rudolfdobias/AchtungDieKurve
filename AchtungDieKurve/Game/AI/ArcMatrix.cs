using System;
using System.Collections.Generic;
using System.Linq;
using AchtungDieKurve.Game.AI.Activities;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.AI
{
    public class ArcMatrix
    {
        public Dictionary<int, PotentialCollision> Arc { get; private set; }

        public AiPlayer Player
        {
            get { return _player; }
        }

        private readonly AiPlayer _player;
        private Vector2 _currentAngle;

        public ArcMatrix(AiPlayer player)
        {
            CreateBlank();
            _player = player;
            _currentAngle = new Vector2();
        }

        public ArcMatrix Load(List<PotentialCollision> items)
        {
            var distanceMatrix = new Dictionary<int, float>();

            foreach (var p in items)
            {
                var distance = Vector2.Distance(_player.CollisionBounds.Origin, p.Origin);
                var targetAngle = Trigonometry.AngleBetween(p.Origin, _player.CollisionBounds.Origin);
                var degTargetAngle = (int) MathHelper.ToDegrees((float) targetAngle);

                if (Math.Abs(MathHelper.WrapAngle((float) targetAngle - _player.Angle)) > Math.PI*0.75f)
                {
                    continue;
                }

                if (distanceMatrix.ContainsKey(degTargetAngle) && distanceMatrix[degTargetAngle] <= distance)
                {
                    continue;
                }
                distanceMatrix[degTargetAngle] = distance;
                Arc[degTargetAngle] = p;
            }
            return this;
        }

        public ArcMatrix Decide(IAiActivity[] activities)
        {
            foreach (var proposal in activities.Select(activity => activity.Propose(this))
                .Where(proposal => proposal.X >= _currentAngle.X))
            {
                _currentAngle = proposal;
            }
            return this;
        }

        private ArcMatrix DebugDecide(IEnumerable<IAiActivity> activities, SpriteBatch sb)
        {
            foreach (var proposal in activities.Select(activity => activity.DrawPropose(this, sb))
                .Where(proposal => proposal.X >= _currentAngle.X))
            {
                _currentAngle = proposal;
            }
            return this;
        }

        public double ProposeAngle()
        {
            return _currentAngle.Y;
        }

        public void DrawDebug(SpriteBatch sb, List<PotentialCollision> items, IAiActivity[] activities)
        {
            Load(items);
            foreach (var p in Arc.Where(p => p.Value != null))
            {
                sb.Draw(CommonResources.whitepixel, p.Value.Bounds, Color.Yellow);
                sb.Draw(CommonResources.whitepixel, new Rectangle(
                    (int) (50*Math.Cos(MathHelper.ToRadians(p.Key))) + _player.ActualBounds.X,
                    (int) (50*Math.Sin(MathHelper.ToRadians(p.Key))) + _player.ActualBounds.Y,
                    1, 1
                    ), Color.White);
            }

            DebugDecide(activities, sb);

            var desiredAngle = ProposeAngle();
            sb.Draw(CommonResources.whitepixel, new Rectangle(
                (int) (Math.Cos(desiredAngle)*50) + _player.ActualBounds.X,
                (int) (Math.Sin(desiredAngle)*50) + _player.ActualBounds.Y,
                3, 3
                ), Color.Red);
        }

        private void CreateBlank()
        {
            Arc = new Dictionary<int, PotentialCollision>();
        }
    }
}