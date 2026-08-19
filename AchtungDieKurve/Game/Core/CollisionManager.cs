using System;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Core
{
    public class CollisionManager
    {
        private readonly GridRegister _register;
        private readonly SpriteBatch _sb;

        public CollisionManager(SpriteBatch sb, ref GridRegister register)
        {
            _register = register;
            Reset();
            _sb = sb;
        }

        public void Carry(ICollidable entity, GameTime gameTime)
        {
            if (entity.CollisionBounds == null) { return; }

            if (!entity.CollisionDisabled)
            {
                _register.Remember(entity);
            }

            if (entity.CanBeHit)
            {
                _register.Find(entity, gameTime, FindCollision);
            }
        }

        public void DebugDraw(ICollidable entity, GameTime gameTime)
        {
            if (GameBase.Defaults.DebugCollisions == false)
                return;

            _register.Draw(gameTime);
            if (entity.CollisionBounds == null) { return; }
            _register.Find(entity, gameTime, DrawCollisionCandidates);

            _register.DrawRadius(entity);

        }

        

        private static bool FindCollision(ICollidable entity, PotentialCollision company, GameTime gameTime)
        {
            var kurve = entity as Kurve;
            if (kurve != null)
            {
                // Playfield boundary handling in Kurve.Update owns wall deaths.
                if (company.Owner is Wall) { return false; }
                if (!KurveOverlaps(kurve, company)) { return false; }
                if (!company.CollisionConditionDelegate(entity, company, gameTime)) { return false; }

                if (company.Owner is Kurve)
                {
                    kurve.SnapToSweepContact(company.Bounds);
                }
                entity.OnCollisionWith(company.Owner, gameTime);
                company.Owner.WasHit(entity, gameTime);
                return true;
            }

            if (company.Bounds.Intersects(entity.CollisionBounds.Bounds) && company.CollisionConditionDelegate(entity, company, gameTime))
            {
                entity.OnCollisionWith(company.Owner, gameTime);
                company.Owner.WasHit(entity, gameTime);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Body circles (matching the round sprite) swept along this frame's
        /// movement, so contact between discrete steps is not missed.
        /// </summary>
        private static bool KurveOverlaps(Kurve kurve, PotentialCollision company)
        {
            var radius = kurve.Diameter / 2f;
            if (company.Owner is Kurve)
            {
                var center = new Vector2(company.Bounds.Center.X, company.Bounds.Center.Y);
                var reach = radius + company.Bounds.Width / 2f;
                return DistancePointToSegment(center, kurve.PreviousPosition, kurve.AbsolutePosition) < reach;
            }

            // powerups and other rectangles: circle vs rectangle at the current position
            var closest = new Vector2(
                MathHelper.Clamp(kurve.AbsolutePosition.X, company.Bounds.Left, company.Bounds.Right),
                MathHelper.Clamp(kurve.AbsolutePosition.Y, company.Bounds.Top, company.Bounds.Bottom));
            return Vector2.DistanceSquared(kurve.AbsolutePosition, closest) < radius * radius;
        }

        private static float DistancePointToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            var ab = b - a;
            var lengthSquared = ab.LengthSquared();
            if (lengthSquared < 1e-6f) { return Vector2.Distance(point, a); }
            var t = MathHelper.Clamp(Vector2.Dot(point - a, ab) / lengthSquared, 0, 1);
            return Vector2.Distance(point, a + ab * t);
        }

        private bool DrawCollisionCandidates(ICollidable entity, PotentialCollision company, GameTime gameTime)
        {
            float multiplier = 1;
            var distance = Vector2.Distance(new Vector2(entity.CollisionBounds.Bounds.Center.X, entity.CollisionBounds.Bounds.Center.Y), new Vector2(company.Bounds.Center.X, company.Bounds.Center.Y));
            if (distance > 0) {
                float ceil = 2 * _register.Raster;
                var one = ceil / 200;
                multiplier = distance / one;
            }

            if (company.Owner is Powerup)
            {
                _sb.Draw(CommonResources.starEffect, company.Bounds, Color.FromNonPremultiplied(240, 240, 255, 200 - (int)multiplier));
            }
            else
            {
                _sb.Draw(company.Owner.BodyTexture, company.Bounds, Color.FromNonPremultiplied(255, 255, 255, 200 - (int)multiplier));    
            }
            
            return false;
        }

        public void Reset()
        {
            _register.Reset();
        }

    }
}
