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
using System.Diagnostics;
using System.Collections.Generic;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace AchtungDieKurve
{
    class GameChoiceScreen : MenuScreen
    {
        #region Fields

        MenuEntry classic, reloaded;
        Properties context;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameChoiceScreen()
            : base("Select game type")
        {
            context = GameBase.Settings;

            classic = new MenuEntry("Classic");
            reloaded = new MenuEntry("Reloaded");


            MenuEntry back = new MenuEntry("Back...");


            classic.Selected += startClassicSelected;
            reloaded.Selected += startReloadedSelected;

            back.Selected += OnCancel;


            MenuEntries.Add(reloaded);
            MenuEntries.Add(classic);
            MenuEntries.Add(back);
        }


        void startClassicSelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.Settings.PowerupsEnabled = false;

            ScreenManager.AddScreen(new GameConfigScreen(), e.PlayerIndex);
            /* LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameConfigScreen());*/
        }

        void startReloadedSelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.Settings.PowerupsEnabled = true;

            ScreenManager.AddScreen(new GameConfigScreen(), e.PlayerIndex);
            /*LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                              new GameConfigScreen());*/
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion
    }
}
