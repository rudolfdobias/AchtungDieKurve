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
using AchtungDieKurve.Screens;
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
        public SfxController SFX;

       private static GameBase instance;
       public static Log Log { get; set; }
        public static GameBase GetInstance()
       {
           return instance;
       }

       public static Properties Defaults;// {get {return context;}}

        
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
            Defaults = new Properties();
            GraphicsManager = new GraphicsManager(this, Defaults);
            
            // Create the screen manager component.
            screenManager = new ScreenManager(this);           
            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            
            screenManager.AddScreen(new MainMenuScreen(), null);
            screenManager.AddScreen(new LogoScreen(), null);

            SFX = new SfxController();
            Defaults.Sfx = SFX;
        }


        private void InitGraphics()
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
            
            base.LoadContent();
            InitGraphics();
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
