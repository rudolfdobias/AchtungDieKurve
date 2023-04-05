#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using AchtungDieKurve.Game;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using AchtungDieKurve.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace AchtungDieKurve.Screens
{

    public class GameplayScreen : GameScreen
    {
        #region Fields

        public event EventHandler Pause;
        public event EventHandler UnPause;

        private ContentManager _content;
        private Properties _context;
        private SpriteBatch _sb;
        private KeyboardState _lastKeystate;
        public Camera2D Camera;
        private List<PlayerDefinition> _playersInfo;
        private float _pauseAlpha;
        private bool _wasPaused;
        // Game components
        private PlayersManager _playersManager;
        private GameInterface _gameLayout;
        private Score _scoreBar;
        private GameMenu _gameMenu;
        private Kurvy _players;
        private PowerupsController _powerups;
        private GridRegister _register;
        private Fps _fps;
        private bool gameTimeUpdated;
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(List<PlayerDefinition> players)
        {
            // defining page transitions
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            // context instance
            _context = GameBase.Settings;

            _playersInfo = players;
            Camera = new Camera2D {Locked = true};
            Pause += TimerPool.Pause;
            UnPause += TimerPool.UnPause;
            gameTimeUpdated = false;
        }

        public PlayersManager PlayersManager
        {
            get { return _playersManager; }
            set { _playersManager = value; }
        }


        public override void LoadContent()
        {
             
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");


            _sb = new SpriteBatch(GameBase.Graphics.GraphicsDevice);      

            ScreenManager.Game.ResetElapsedTime();

            _gameLayout = new GameInterface(GameBase.GetInstance(), _sb);
            GameBase.Log = new Log(GameBase.GetInstance(), _sb, _gameLayout.PlayableArea);
            _register = new GridRegister(_sb, _gameLayout.PlayableArea, 64);
            _gameMenu = new GameMenu(_gameLayout.MenuContainer, GameBase.GetInstance(), _sb);
            _playersManager = new PlayersManager(GameBase.GetInstance());
            _players = new Kurvy(_gameLayout.PlayableArea, _playersManager, GameBase.GetInstance(), _sb, this, ref _register);
            _scoreBar = new Score(_gameLayout.LeftBar, _playersManager.Worms, GameBase.GetInstance(), _sb);
            _playersManager.AddPlayers(_playersInfo, _gameLayout.PlayableArea);
            _playersManager.Win += Victory;
            _players.LoadContent();
            _playersManager.SetAndStart();
            _fps = new Fps(GameBase.GetInstance(), _sb, _gameLayout.PlayableArea);
            ResetRoundState();
            
        }


        public void Victory(Kurve winner)
        {
            Thread.Sleep(2500);
            LoadingScreen.Load(ScreenManager, true, null,
                   new ScoreScreen(_playersManager));
        }


        public override void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

           
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            _pauseAlpha = coveredByOtherScreen ? Math.Min(_pauseAlpha + 1f / 32, 1.5f) : Math.Max(_pauseAlpha - 1f / 32, 0);
            
            if (IsActive)
            {
                if (_wasPaused)
                {
                    _wasPaused = false;
                    UnPause?.Invoke(this, new EventArgs());
                }
                _gameLayout.Update(gameTime);
                GameBase.Log.Update(gameTime);
                _scoreBar.Update(gameTime);
                _gameMenu.Update(gameTime);
                _players.Update(gameTime);
                _fps.Update(gameTime);
                
            }
            else
            {
                _wasPaused = true;
                if (Pause != null) { Pause(this, new EventArgs());}
            }

            Camera.Update();
           
        }

        /// <summary> handles all universal game input e.g. pausing  </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            
            // Look up inputs for the active player profile.
            var playerIndex = (int)ControllingPlayer.Value;
            var keyboardState = input.CurrentKeyboardStates[playerIndex];

            if (keyboardState.IsKeyDown(Keys.F12) && !_lastKeystate.IsKeyDown(Keys.F12))
            {
                if (_context.Sfx.SoundEnabled())
                    _context.Sfx.DisableSound();
                else
                    _context.Sfx.EnableSound();
            }

            if (input.IsPauseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

            _lastKeystate = keyboardState;

        }
      
        /// <summary> draws the HUD (score and round) ans the lines to the screen </summary>
        public override void Draw(GameTime gameTime)
        {    
            _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, null, null, Camera.Transform);

            _gameLayout.Draw(gameTime);
            GameBase.Log.Draw(gameTime);
            _scoreBar.Draw(gameTime);
            _gameMenu.Draw(gameTime);
            _sb.End();
            _sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, null, null, _gameLayout.PlayableArea.offsetTransform(Camera.GetSpecialCamera()));
            _players.Draw(gameTime);
            _fps.Draw(gameTime);
            _sb.End();

            if (!(TransitionPosition > 0) && !(_pauseAlpha > 0)) return;
            var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
            ScreenManager.FadeBackBufferToBlack(alpha);
        }

        public void ResetRoundState()
        {
            _players.Reset();
            _playersManager.Reset();
        }
     
    }
}
