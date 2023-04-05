using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class Fast : Powerup
    {
        public override PowerupType Type
        {
            get { return PowerupType.ForMe; }
        }

        public const float SpeedAddition = 1.2f;
        private float _lastSpeed = 1.2f;

        public Fast(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Fast(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 4000; }
        }

        protected override string TextureName
        {
            get { return "fast_green"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            var delta = Invoker.Speed + SpeedAddition;
            Invoker.Speed += SpeedAddition;
            if (delta > Invoker.Speed)
            {
                _lastSpeed = SpeedAddition - (delta - Invoker.Speed);
            }
            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.Speed -= _lastSpeed;
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
