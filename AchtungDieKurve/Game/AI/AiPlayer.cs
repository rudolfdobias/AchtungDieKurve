using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.AI
{
    
    public class AiPlayer : Player
    {
        public event ControlEvent Controlling;

        public AiPlayer(Properties context, Keys left, Keys right, Color colour, IContainer container, bool isAI = false)
            : base(context, left, right, colour, container, isAI)
        {


        }

        protected override void Control(KeyboardState keyboardState, GameTime gameTime)
        {
            if (Controlling != null) { Controlling(this, keyboardState, gameTime); }
        }

        public void TurnRight()
        {
            Angle += TurnStep;
        }

        public void TurnLeft()
        {
            Angle -= TurnStep;
        }

        public override void Reset()
        {
            base.Reset();
            //Speed = 1.5f;
            //this.AbsolutePosition = new Vector2((float)(container.ContentArea.Width / 2), (float)(container.ContentArea.Height / 2));
        }
    }
}
