using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.Drawable
{
    public class Player : Kurve
    {
        public Player(Properties context, Keys left, Keys right, Color colour, IContainer container, bool isAi = false)
        {
            Left = left;
            Right = right;
            Color = colour;
            _ai = isAi;
            Container = container;
            SetContext(context);
        }
    }
}
