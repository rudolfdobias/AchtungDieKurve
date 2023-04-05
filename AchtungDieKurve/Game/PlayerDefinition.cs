using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game
{
    public class PlayerDefinition
    {
        public PlayerDefinition(string name, Keys left, Keys right, Color color, bool ai = false)
        {
            Right = right;
            Left = left;
            Name = name;
            Color = color;
            IsAi = ai;
        }

        public Keys Left;
        public Keys Right;
        public Color Color;
        public string Name;
        public bool Active = false;
        public Vector2 Position = Vector2.Zero;

        public bool IsAi { get; set; }

    }
}