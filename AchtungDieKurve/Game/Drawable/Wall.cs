using System;
using AchtungDieKurve.Game.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    public class Wall : DefaultDrawable, ICollidable
    {
        public event CollidableObjectMoved Move;
        private readonly Texture2D _texture;
        private readonly Rectangle _bounds;

        public Wall(int X, int Y, int W, int H, GameBase game, SpriteBatch SB):base(game, SB)
        {
            _texture = CommonResources.whitepixel;
            _bounds = new Rectangle(X, Y, W, H);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _bounds, Color.Blue);
        }

        public Texture2D BodyTexture
        {
            get
            {
                return _texture;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public PotentialCollision CollisionBounds
        {
            get {
                return new PotentialCollision(this, _bounds, CollidableShape.Rectangle, Condition);
            }
        }

        public bool CollisionDisabled
        {
            get { return false; }
        }

        public void OnCollisionWith(ICollidable entity, GameTime gameTime)
        {
            
        }

        public void WasHit(ICollidable entity, GameTime gameTime)
        {
            // hello kitty
        }

        public bool Condition(ICollidable me, PotentialCollision company, GameTime gameTime)
        {
            return true;
        }
    }
}
