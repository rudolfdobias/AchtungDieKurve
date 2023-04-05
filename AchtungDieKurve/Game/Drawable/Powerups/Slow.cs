using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class Slow : Powerup
    {
        public override PowerupType Type { get { return PowerupType.ForMe;} }
        public const float SppedFactor = 1.5f;
        private float _lastSpeed;

        public Slow(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Slow(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 6000; }
        }

        protected override string TextureName
        {
            get { return "slow_green"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            _lastSpeed = Invoker.Speed;
            Invoker.Speed /= SppedFactor;
            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.Speed = _lastSpeed;
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
