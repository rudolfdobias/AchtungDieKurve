using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    class Clear : Powerup
    {
        public Clear(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Clear(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
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
            get { return "clear"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            Gameplay.ResetRoundState();
        }

        public override void Undo()
        {
            // what the fuck you think I should do here.
        }
    }
}
