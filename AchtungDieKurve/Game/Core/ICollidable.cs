using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Core
{
    public interface ICollidable
    {
        event CollidableObjectMoved Move;

        Texture2D BodyTexture { get; set; }
        PotentialCollision CollisionBounds { get; }

        /// <summary>True when the entity must not be registered into the collision grid (e.g. while drawing a hole).</summary>
        bool CollisionDisabled { get; }

        /// <summary>True when the entity can collide with others (die, collect) right now.</summary>
        bool CanBeHit { get; }


        void OnCollisionWith(ICollidable entity, GameTime gameTime);
        void WasHit(ICollidable entity, GameTime gameTime);
    }

    public enum CollidableShape { Rectangle, Circle }

    public delegate void CollidableObjectMoved(ICollidable entity, GameTime gameTime);
}