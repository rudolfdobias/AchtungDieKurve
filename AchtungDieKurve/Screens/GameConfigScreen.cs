using System;
using System.Collections.Generic;
using System.Linq;
using AchtungDieKurve.Game;
using AchtungDieKurve.Game.Drawable.Parts.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Screens
{
    internal class GameConfigScreen : MenuScreen
    {
        private GameConfigBox _configBox;
        public List<PlayerDefinition> Players = new List<PlayerDefinition>();
        private MenuEntry _play;
        private MenuEntry _change;
        private MenuEntry _back;


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameConfigScreen()
            : base("Game setup")
        {
            LoadMenuEntries(); 
            
        }
        

        private void InitPlayers()
        {
            var definitions = PlayersManager.GetPlayerDefinitions();
            for (var i = 0; i < definitions.Count; i++)
            {
                var info = definitions[i];
                Players.Add(info);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            
            
            InitPlayers();
            InitBox();
        }

        private void LoadMenuEntries()
        {
            var start = new MenuEntry("Play!");
            var change = new MenuEntry("Change");
            var back = new MenuEntry("Back");
            
            start.Selected += StartSelected;
            back.Selected += OnCancel;
            change.Selected += ChangeOnSelected;
            MenuEntries.Add(start);
            MenuEntries.Add(change);
            MenuEntries.Add(back);
        }

        private void ChangeOnSelected(object sender, PlayerIndexEventArgs playerIndexEventArgs)
        {
            _configBox.Active = !_configBox.Active;
        }

        private void InitBox()
        {
            _configBox = new GameConfigBox(GameBase.GetInstance(), ScreenManager.SpriteBatch, ref Players, this);
        }

        void StartSelected(object sender, PlayerIndexEventArgs e)
        {
            if (Players.Sum(definition => definition.Active == true ? 1 : 0) < 2)
            {
                return;
            }
            //context.gameReset(new GameTime());
            List<PlayerDefinition> players = (from p in Players where p.Active select p).ToList();

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                new GameplayScreen(players));

            //SetMenuEntryText();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            var spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            _configBox.Draw(gameTime);
            spriteBatch.End();
            
        }

        

        public override void HandleInput(InputState input)
        {
            PlayerIndex player;
            if (_configBox.Active)
            {
                _configBox.HandleInput(input);
            }
            else
            {
                foreach (var P in Players)
                {
                    
                    if (input.IsNewKeyPress(P.Left, null, out player) ||
                        input.IsNewKeyPress(P.Right, null, out player))
                    {
                        P.Active = !P.Active;
                        P.IsAi = false;

                    }
                    var playerIndex = (int)ControllingPlayer.Value;
                    var keyState = input.CurrentKeyboardStates[playerIndex];
                    var keys = keyState.GetPressedKeys();
                    foreach (var value in keys.Where(k => k >= Keys.D0 && k <= Keys.D9).Select(k => (int)k - 48).Where(value => value > 0 && value < Players.Count))
                    {
                        Players[value].IsAi = true;
                        Players[value].Active = true;
                    }
                }
                base.HandleInput(input);
            }

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (_configBox.Active)
            {

            }
            else
            {
                
            }
        }

      

    }
}
