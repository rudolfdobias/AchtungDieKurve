using System;
using System.Collections.Generic;
using AchtungDieKurve.Game.AI;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.Drawable
{
    class Kurvy : DefaultDrawable, IContainer
    {
        private readonly IContainer _container;
        private readonly PlayersManager _players;
        private readonly CollisionManager _collisionManager;
        private readonly AiDriver _driver;
        private readonly GridRegister _register;
        private readonly List<DefaultDrawable> _statics;
        private readonly PowerupsController _powerups;

        public Kurvy(IContainer container, PlayersManager players, GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, ref GridRegister register)
            :base(game, spriteBatch){
                _container = container;
                _players = players;
                _players.PlayerAdded += players_PlayerAdded;
                _players.NextRound += players_NextRound;
                _statics = CreateWalls();
                _register = register;
                _collisionManager = new CollisionManager(spriteBatch, ref _register);
                _driver = new AiDriver(container, ref _register);
                _powerups = new PowerupsController(GameBase.GetInstance(), spriteBatch, container, ref _register, gameplay);
                RegisterWalls();
                
        }

        void players_NextRound(object sender, EventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            _collisionManager.Reset();
            RegisterWalls();
            _powerups.Reset();
            foreach (var player in _players.Worms)
            {
                player.Reset();
                player.Live();
            }
        }

        void RegisterWalls()
        {
            foreach (var block in _statics)
            {
                _register.Remember(block as ICollidable);
            }
        }

        void players_PlayerAdded(Kurve k)
        {
            k.Move += _collisionManager.Carry;
            k.Drawed += _collisionManager.DebugDraw;

            var player = k as AiPlayer;
            if (player != null)
            {
                player.Controlling += _driver.ControlAi;
            }
        }
       
        public Rectangle ContentArea
        {
            get
            {
                return _container.ContentArea;
            }
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var k in _players.Worms)
            {
                k.Update(Keyboard.GetState(), gameTime);
            }
            _powerups.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            /*foreach (var block in _statics)
            {
                block.Draw(gameTime);
            }*/
            foreach (var k in _players.Worms)
            {
                k.Draw(gameTime, spriteBatch);
                _driver.DrawDebug(spriteBatch, k as AiPlayer, gameTime);
            }
            // front layer
            foreach (var k in _players.Worms)
            {
                k.DrawPowerupStats(spriteBatch, gameTime);
                
            }

            _powerups.Draw(gameTime);

            DrawDebug();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            foreach (var k in _players.Worms)
            {
                k.LoadContent(game.Content);
            }
            _powerups.LoadContent();
        }

        private void DrawDebug()
        {
            if (false == GameBase.Defaults.DebugCoordinates)
                return;

            var y = ContentArea.Y + 10;
            var x = ContentArea.X + 10;
            foreach (var k in _players.Worms)
            {
                spriteBatch.DrawString(
                    CommonResources.fontSmaller,
                    Math.Floor(k.AbsolutePosition.X) + " / " + Math.Floor(k.AbsolutePosition.Y) + $"({(k.IsAlive ? "alive" : "dead")})",
                    new Vector2(x, y),
                    k.Color
                    );

                y += 20;
            }
        }

        private List<DefaultDrawable> CreateWalls()
        {
            var walls = new List<DefaultDrawable>
            {
                new Wall(0, 0, ContentArea.Width, 5, game, spriteBatch),
                new Wall(0, 0, 5, ContentArea.Height, game, spriteBatch),
                new Wall(ContentArea.Width, 0, 5, ContentArea.Height, game, spriteBatch),
                new Wall(0, ContentArea.Height, ContentArea.Width, 5, game, spriteBatch)
            };
            return walls;
        }
        
    }
}
