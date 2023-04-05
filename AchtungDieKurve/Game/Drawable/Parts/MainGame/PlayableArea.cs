using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Parts.MainGame
{
    public sealed class PlayableArea : IContainer
    {
        const int MARGIN_LEFT = 10;
        const int MARGIN_TOP = 10;
        const int MARGIN_RIGHT = 10;
        const int MARGIN_BOTTOM = 10;
        private Rectangle container;
        private Rectangle PlayableAreaWhiteBorder;

        public PlayableArea(Rectangle container)
        {
            this.container = container;
            PlayableAreaWhiteBorder = new Rectangle(this.ContentArea.Left - 1, this.ContentArea.Top - 1, this.ContentArea.Width + 2, this.ContentArea.Height + 2);
        }
        public Rectangle ContentArea
        {
            get {
                return new Rectangle(container.Left + MARGIN_LEFT, container.Top + MARGIN_TOP, container.Width - MARGIN_RIGHT - MARGIN_LEFT, container.Height - MARGIN_BOTTOM - MARGIN_TOP);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch SB)
        {
            SB.Draw(CommonResources.whitepixel, PlayableAreaWhiteBorder, CommonResources.Borders);
            SB.Draw(CommonResources.whitepixel, this.ContentArea, Color.Black);
        }

        public Matrix offsetTransform(Matrix matrix)
        {
            return Matrix.CreateTranslation(ContentArea.X, ContentArea.Y, 0);
        }
    }
}
