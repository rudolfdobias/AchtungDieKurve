#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace AchtungDieKurve.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class LogoScreen : GameScreen
    {
        private TimeSpan _showStart;
        private readonly TimeSpan _displayTime = TimeSpan.FromSeconds(3);
        private SpriteBatch _batch;

        private const float LogoScreenWPercent = 0.4f;
        private const float LogoWidthHeightRatio = 0.374f;

        private Texture2D _logo;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public LogoScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(1);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            
            if (_showStart == TimeSpan.Zero)
                _showStart = gameTime.TotalGameTime;

            if (gameTime.TotalGameTime >= _showStart + _displayTime && !IsExiting)
            {
                ExitScreen();
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            var screen = ScreenManager.GraphicsDevice.Viewport.Bounds;
            var logoW = (int)(screen.Size.X * LogoScreenWPercent);
            var logoH = (int)(logoW * LogoWidthHeightRatio);
            var posX = (int)(screen.Size.X - logoW) /2;
            var posY = (int)(screen.Size.Y - logoH) /2;
            
            var fadeColor = Color.White;
            fadeColor *= TransitionAlpha;
            
            _batch.Begin();
            _batch.Draw(_logo, new Rectangle(posX, posY, logoW,logoH), fadeColor);
            _batch.End();
            
            base.Draw(gameTime);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _batch = new SpriteBatch(ScreenManager.GraphicsDevice);
            var content = new ContentManager(ScreenManager.Game.Services, "Content");
            _logo = content.Load<Texture2D>("ADKLogo2");
        }
    }
}
