using AchtungDieKurve.Game.Drawable.Parts.MainGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game.Drawable
{
    class GameInterface : DefaultDrawable, IContainer
    {
        const int MAIN_FRAME_LEFT = 50;
        const int MAIN_FRAME_TOP = 10;
        const int MAIN_FRAME_RIGHT = 10;
        const int MAIN_FRAME_BOTTOM = 10;

        public PlayableArea PlayableArea;
        public LeftBar LeftBar;
        public Menu MenuContainer;

        public GameInterface(GameBase game, SpriteBatch spriteBatch)
            :base(game, spriteBatch){
                
        }

        public Rectangle ContentArea
        {
            get
            {
                return new Rectangle(0,0,GameBase.Settings.ScreenWidth, GameBase.Settings.ScreenHeight);
            }
        }

        protected override void Init()
        {
            base.Init();
            defineLayout();
        }

        private void defineLayout()
        {
            LeftBar = new LeftBar();
            PlayableArea = new Parts.MainGame.PlayableArea(new Rectangle(LeftBar.ContentArea.Width, 0, this.ContentArea.Width - LeftBar.ContentArea.Width, this.ContentArea.Height));
            MenuContainer = new Menu(LeftBar);
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(CommonResources.whitepixel, this.ContentArea, CommonResources.Background);
            LeftBar.Draw(gameTime, spriteBatch);
            MenuContainer.Draw(gameTime, spriteBatch);
            PlayableArea.Draw(gameTime, spriteBatch);
        }

        
    }
}
