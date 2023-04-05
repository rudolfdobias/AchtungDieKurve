using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    class Death : Powerup
    {
        public Death(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Death(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override PowerupType Type
        {
            get { return PowerupType.Neutral; }
        }

        public override int DurationMilliseconds
        {
            get { return 1; }
        }

        protected override string TextureName
        {
            get { return "grave"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            Invoker.Kill(gameTime);
        }

        public override void Undo()
        {
            // hehe.
        }
    }
}
