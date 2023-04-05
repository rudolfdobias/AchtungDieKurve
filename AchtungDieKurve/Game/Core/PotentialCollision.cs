using Microsoft.Xna.Framework;

namespace AchtungDieKurve.Game.Core
{
    public class PotentialCollision
    {
        public CollidableShape Type;
        public ICollidable Owner;
        public CollisionCondition CollisionCondition;
        public Rectangle Bounds;
        private Vector2 segment;
        private bool isSegment;

        public PotentialCollision(ICollidable owner, Rectangle bounds, CollidableShape type, CollisionCondition anotherCondition)
        {
            Owner = owner;
            Bounds = bounds;
            Type= type;
            CollisionCondition = anotherCondition;
        }

        public PotentialCollision CreateSegment(Vector2 position)
        {
            return new PotentialCollision(Owner, Bounds, Type, CollisionCondition).SetSetgment(position);
        }

        public PotentialCollision SetSetgment(Vector2 position)
        {
            segment = position;
            isSegment = true;
            return this;
        }

        public Vector2 Origin
        {
            get
            {
                return !isSegment ? new Vector2(Bounds.X, Bounds.Y) : segment;
            }
        }

        public Vector2 Center
        {
            get
            {
                return !isSegment ? new Vector2(Bounds.Center.X, Bounds.Center.Y) : segment;
            }
        }

    }

    public delegate bool CollisionCondition(ICollidable k, PotentialCollision collidingWith, GameTime gameTime);
}