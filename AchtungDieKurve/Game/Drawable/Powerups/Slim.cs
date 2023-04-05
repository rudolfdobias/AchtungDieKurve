using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class Slim : Powerup
    {
        public override PowerupType Type { get { return PowerupType.ForMe;} }
        public const int ThicknessFactor = 3;
        public const float AngleFactor = 1.5f;
        private int _thicknessReverse = 3;

        public Slim(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Slim(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 7000; }
        }

        protected override string TextureName
        {
            get { return "thin_green"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            var prev = Invoker.Diameter;
            Invoker.Diameter -= ThicknessFactor;
            if (prev - Invoker.Diameter > Invoker.Diameter - ThicknessFactor)
            {
                _thicknessReverse = prev - Invoker.Diameter;
            }
           
            Invoker.TurnStep *= AngleFactor;
            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.Diameter += _thicknessReverse;
            Invoker.TurnStep /= AngleFactor;
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
