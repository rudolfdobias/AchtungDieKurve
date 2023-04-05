using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    class GameMenu : DefaultDrawable
    {
        const int MENU_CELL_SPACING = 10;
        const int MENU_CELL_HEIGHT = 15;
        const int MENU_PARAGRAPH_OFFSET = 5;

        private IContainer container;

        public GameMenu(IContainer container, GameBase game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            this.container = container;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            Rectangle placement = new Rectangle();
            placement.X = this.container.ContentArea.X;
            placement.Y = this.container.ContentArea.Y;
            placement.Width = container.ContentArea.Width;
            placement.Height = MENU_CELL_HEIGHT;

            // sound
            bool soundEnabled = this.game.SFX.SoundEnabled();
            string soundState = soundEnabled ? "(on)" : "(off)";
            string soundKey = "[F12]";
            string soundText = "Sound";

            spriteBatch.DrawString(
             CommonResources.fontSmaller,
             soundKey,
             new Vector2(
                 placement.X,
                 placement.Y
                 ),
                 CommonResources.Emhasis
                 );

            placement.Y += MENU_CELL_HEIGHT;
            spriteBatch.DrawString(
            CommonResources.fontSmaller,
            soundText,
            new Vector2(placement.X + MENU_PARAGRAPH_OFFSET, placement.Y),
                CommonResources.Borders
                );

            placement.Y += MENU_CELL_HEIGHT;

            spriteBatch.DrawString(
            CommonResources.fontSmaller,
            soundState,
            new Vector2(placement.X + MENU_PARAGRAPH_OFFSET, placement.Y),
                CommonResources.Borders
                );


            placement.Y += MENU_CELL_HEIGHT + MENU_CELL_SPACING;

            // pause
            string pausekey = "[ESC]";
            string pauseText = "Pause";
            spriteBatch.DrawString(
            CommonResources.fontSmaller,
            pausekey,
            new Vector2(
                placement.X,
                placement.Y
                ),
                CommonResources.Emhasis
                );
            placement.Y += MENU_CELL_HEIGHT;

            spriteBatch.DrawString(
            CommonResources.fontSmaller,
            pauseText,
            new Vector2(placement.X + MENU_PARAGRAPH_OFFSET, placement.Y),
                CommonResources.Borders
                );
        }

       
    }
}
