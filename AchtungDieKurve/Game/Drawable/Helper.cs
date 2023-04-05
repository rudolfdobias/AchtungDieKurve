using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game.Drawable
{
    public class Helper
    {
        public static Vector2 CenterText(SpriteFont font, string text, Rectangle container)
        {
            Vector2 textDimensions = font.MeasureString(text);
            return new Vector2(
                (float)Math.Floor(container.X + (container.Width - (textDimensions.X / 2)) / 2),
                (float)Math.Floor(container.Y + (container.Height - (textDimensions.Y / 2)) / 2)
                );
        }
    }
}
