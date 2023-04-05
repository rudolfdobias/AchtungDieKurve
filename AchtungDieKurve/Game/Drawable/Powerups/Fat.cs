using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class Fat : Powerup
    {
        public override PowerupType Type
        {
            get { return PowerupType.ForMe; }
        }

        public const int ThicknessFactor = 5;
        private int _thicknessReverse = 5;

        public Fat(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Fat(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 7000; }
        }

        protected override string TextureName
        {
            get { return "fat_green"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            var prev = Invoker.Diameter;
            Invoker.Diameter += ThicknessFactor;

            if (Invoker.Diameter - prev < ThicknessFactor + ThicknessFactor)
            {
                _thicknessReverse = Invoker.Diameter - prev;
            }


            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.Diameter -= _thicknessReverse;
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
