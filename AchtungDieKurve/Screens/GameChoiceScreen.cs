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
            context = GameBase.Defaults;

            classic = new MenuEntry("Classic");
            reloaded = new MenuEntry("Boosted");


            MenuEntry s1 = new MenuEntry("");
            MenuEntry back = new MenuEntry("Back");


            classic.Selected += StartClassicSelected;
            reloaded.Selected += StartReloadedSelected;

            back.Selected += OnCancel;


            MenuEntries.Add(reloaded);
            MenuEntries.Add(classic);
            //MenuEntries.Add(s1);
            MenuEntries.Add(back);
        }


        void StartClassicSelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.Defaults.PowerupsEnabled = false;

            ScreenManager.AddScreen(new GameConfigScreen(), e.PlayerIndex);
            /* LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameConfigScreen());*/
        }

        void StartReloadedSelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.Defaults.PowerupsEnabled = true;

            ScreenManager.AddScreen(new GameConfigScreen(), e.PlayerIndex);
            /*LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                              new GameConfigScreen());*/
        }

        #endregion
        
    }
}
