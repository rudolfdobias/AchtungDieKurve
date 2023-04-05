#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AchtungDieKurve
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class VideoScreen : MenuScreen
    {
        #region Fields

        MenuEntry adapterEntry;
        MenuEntry fullscreenEntry;
        MenuEntry resolutionEntry;
        MenuEntry applyEntry;
        MenuEntry separator;

        static bool fullscreen = false;
        static bool lowRes = false;
  

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public VideoScreen(ScreenManager ScreenManager)
            : base("Video")
        {
            // Create our menu entries.
            adapterEntry = new MenuEntry(string.Empty);
            resolutionEntry = new MenuEntry(string.Empty);
            fullscreenEntry = new MenuEntry(string.Empty);
            applyEntry= new MenuEntry(string.Empty);
            separator = new MenuEntry(string.Empty);

            this.ScreenManager = ScreenManager;
            MenuEntry back = new MenuEntry("Back");

           

            // Hook up menu event handlers.
            adapterEntry.Selected += adapterEntrySelected;
            resolutionEntry.Selected += resolutionEntrySelected;
            fullscreenEntry.Selected += fullscreenEntrySelected;
            applyEntry.Selected += applyEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(adapterEntry);
            MenuEntries.Add(resolutionEntry);
            MenuEntries.Add(fullscreenEntry);
          //  MenuEntries.Add(separator);
          //  MenuEntries.Add(applyEntry);
            MenuEntries.Add(separator);
            MenuEntries.Add(back);

            SetMenuEntryText();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        public void SetMenuEntryText()
        {

            adapterEntry.Text = ScreenManager.GraphicsDevice.Adapter.Description;
            fullscreenEntry.Text = "Fullscreen: " + (fullscreen ? "ON" : "OFF");
            resolutionEntry.Text = GameBase.Graphics.PreferredBackBufferWidth + " x " + GameBase.Graphics.PreferredBackBufferHeight.ToString();
            applyEntry.Text = "Apply resolution";
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void adapterEntrySelected(object sender, PlayerIndexEventArgs e)
        {
        
            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void resolutionEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            lowRes = !lowRes;

            // todo: make it a little bit more inteligent... but it's sufficient for this night..

            if (lowRes == false)
            {
                GameBase.Graphics.PreferredBackBufferWidth = 2560;
                GameBase.Graphics.PreferredBackBufferHeight = 1440;
                GameBase.Settings.DefaultDiameter = 16;
                GameBase.Settings.DefaultSpeed = 3f;
            }
            else
            
            {
                GameBase.Graphics.PreferredBackBufferWidth = 1920;
                GameBase.Graphics.PreferredBackBufferHeight = 1080;
                GameBase.Settings.DefaultDiameter = 8;
                GameBase.Settings.DefaultSpeed = 2.2f                                   ;
            }
            GameBase.Graphics.ApplyChanges();
            if (GameBase.Graphics.IsFullScreen)
            {
                GameBase.Settings.ScreenWidth = GameBase.Graphics.GraphicsDevice.DisplayMode.Width;
                GameBase.Settings.ScreenHeight = GameBase.Graphics.GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                GameBase.Settings.ScreenWidth = GameBase.Graphics.GraphicsDevice.Viewport.Width;
                GameBase.Settings.ScreenHeight = GameBase.Graphics.GraphicsDevice.Viewport.Height;
            }
            
            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void fullscreenEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            fullscreen = !fullscreen;
            GameBase.Graphics.ToggleFullScreen();
            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void applyEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.Graphics.ApplyChanges();

            SetMenuEntryText();
        }


        #endregion
    }
}
