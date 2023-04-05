using System.Xml;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class NoWalls : Powerup
    {
        public override PowerupType Type
        {
            get { return PowerupType.ForMe; }
        }



        public NoWalls(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public NoWalls(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
        }

        public override int DurationMilliseconds
        {
            get { return 8000; }
        }

        protected override string TextureName
        {
            get { return "nowalls"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            Invoker.WallTraversalEnabled = true;
            //Invoker._hasProtection = true;
            Invoker.ActivePowerups.Add(this);
        }

        public override void Undo()
        {
            Invoker.WallTraversalEnabled = false;
            //Invoker._hasProtection = false;
            Invoker.ActivePowerups.Remove(this);
        }
    }
}
