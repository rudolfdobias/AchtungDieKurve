using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Core
{
    public interface ICollidable
    {
        event CollidableObjectMoved Move;

        Texture2D BodyTexture { get; set; }
        PotentialCollision CollisionBounds { get; }
        bool CollisionDisabled { get; }


        void OnCollisionWith(ICollidable entity, GameTime gameTime);
        void WasHit(ICollidable entity, GameTime gameTime);
    }

    public enum CollidableShape { Rectangle, Circle }

    public delegate void CollidableObjectMoved(ICollidable entity, GameTime gameTime);
}