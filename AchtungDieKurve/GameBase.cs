#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using AchtungDieKurve.Sound;
using AchtungDieKurve.Game;
using AchtungDieKurve.Graphics;
using System;
using AchtungDieKurve.Game.Drawable;
using Color = Microsoft.Xna.Framework.Color;

#endregion

namespace AchtungDieKurve
{

    public class GameBase : Microsoft.Xna.Framework.Game
    {
        #region Fields

        public static GraphicsDeviceManager Graphics;
        public static GraphicsManager GraphicsManager;
        ScreenManager screenManager;
        public static SoundEffect crash;
        public Properties settings;
        public SfxController SFX;

       private static GameBase instance;
        public static TimeSpan LastGameStarted { get; set; }
        public static Log Log { get; set; }
        public static GameBase GetInstance()
       {
           return instance;
       }

       public static Properties Settings;// {get {return context;}}

        
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public GameBase()
        {
            instance = this;
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            Settings = new Properties();
            GraphicsManager = new GraphicsManager(this, Settings);
            this.configureGraphics();
            // Create the screen manager component.
            screenManager = new ScreenManager(this);           
            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            SFX = new SfxController();
            Settings.Sfx = SFX;
        }


        private void configureGraphics()
        {
            //todo: loading saved settings
            GraphicsManager.Reset();
        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            SFX.Load();
            CommonResources.Load(this);
        }
       

        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.Black);
           
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

       
        #endregion
    }
}
