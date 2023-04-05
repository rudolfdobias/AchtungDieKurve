using System;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Core
{
    public class CollisionManager
    {
        
        protected GridRegister Register;
        protected SpriteBatch Sb;

        public CollisionManager(SpriteBatch sb, ref GridRegister register)
        {
            Register = register;
            Reset();
            Sb = sb;
        }

        public void Carry(ICollidable entity, GameTime gameTime)
        {
            if (entity.CollisionDisabled || entity.CollisionBounds == null) { return; }
            
            
            if (entity.CollisionBounds != null)
            {
                Register.Remember(entity);
            }

            Register.Find(entity, gameTime, FindCollision);
        }

        public void DebugDraw(ICollidable entity, GameTime gameTime)
        {
            if (GameBase.Settings.DebugCollisions == false)
                return;

            Register.Draw(gameTime);
            if (entity.CollisionBounds == null) { return; }
            Register.Find(entity, gameTime, DrawCollisionCandidates);

            Register.DrawRadius(entity);

        }

        

        private static bool FindCollision(ICollidable entity, PotentialCollision company, GameTime gameTime)
        {
            switch (company.Type)
            {
                case CollidableShape.Rectangle:
                    
                    if (company.Bounds.Intersects(entity.CollisionBounds.Bounds) && company.CollisionCondition(entity, company, gameTime))
                    {
                        entity.OnCollisionWith(company.Owner, gameTime);
                        company.Owner.WasHit(entity, gameTime);
                        return true;
                    }
                    break;
                case CollidableShape.Circle:
                    break;
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
                float ceil = 2 * Register.Raster;
                var one = ceil / 200;
                multiplier = distance / one;
            }

            if (company.Owner is Powerup)
            {
                Sb.Draw(CommonResources.starEffect, company.Bounds, Color.FromNonPremultiplied(240, 240, 255, 200 - (int)multiplier));
            }
            else
            {
                Sb.Draw(company.Owner.BodyTexture, company.Bounds, Color.FromNonPremultiplied(255, 255, 255, 200 - (int)multiplier));    
            }
            
            return false;
        }

        public void Reset()
        {
            Register.Reset();
        }

    }
}
