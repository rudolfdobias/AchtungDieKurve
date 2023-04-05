using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game.Drawable.Parts.MainGame
{
    public sealed class LeftBar : IContainer
    {
        const int WIDTH = 50;
        public Microsoft.Xna.Framework.Rectangle ContentArea
        {
            get {
                return new Microsoft.Xna.Framework.Rectangle(0, 0, WIDTH, GameBase.Settings.ScreenHeight);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch SB)
        {

        }
    }
}
