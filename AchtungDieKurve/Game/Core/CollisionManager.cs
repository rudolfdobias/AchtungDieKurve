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
            if (entity.CollisionDisabled || entity.CollisionBounds == null) { return; }
            
            
            if (entity.CollisionBounds != null)
            {
                _register.Remember(entity);
            }

            _register.Find(entity, gameTime, FindCollision);
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
            switch (company.Type)
            {
                case CollidableShape.Rectangle:
                    
                    if (company.Bounds.Intersects(entity.CollisionBounds.Bounds) && company.CollisionConditionDelegate(entity, company, gameTime))
                    {
                        entity.OnCollisionWith(company.Owner, gameTime);
                        company.Owner.WasHit(entity, gameTime);
                        return true;
                    }
                    break;
                case CollidableShape.Circle:
                //break;
                default:
                    throw new NotImplementedException("Cannot detect collision for type " + company.Type);
            }
            return false;
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
