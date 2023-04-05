using System.Xml;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class Transcend : Powerup
    {
        public override PowerupType Type
        {
            get { return PowerupType.ForMe; }
        }



        public Transcend(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Transcend(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 5000; }
        }

        protected override string TextureName
        {
            get { return "transcend"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            entity.StartProtection(DurationMilliseconds);
            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.TriggerEndProtection();
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
