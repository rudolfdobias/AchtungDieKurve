using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game.Drawable.Parts.MainGame
{
    public sealed class Menu : IContainer
    {
        const int HEIGHT = 100;
        const int MARGIN_LEFT = 7;
        const int MARGIN_TOP = 0;
        const int MARGIN_RIGHT = 0;
        const int MARGIN_BOTTOM = 0;

        private IContainer container;
        public Menu(IContainer container)
        {
            this.container = container;
        }

        public Microsoft.Xna.Framework.Rectangle ContentArea
        {
            get {
                return new Microsoft.Xna.Framework.Rectangle(container.ContentArea.Left + MARGIN_LEFT, container.ContentArea.Bottom - HEIGHT + MARGIN_TOP, container.ContentArea.Right - MARGIN_RIGHT, container.ContentArea.Bottom - MARGIN_BOTTOM);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch SB)
        {
           // SB.Draw(CommonResources.whitepixel, this.container.ContentArea, Color.Black);
        }
    }
}
