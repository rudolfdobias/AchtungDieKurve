using System;
using System.Collections.Generic;
using System.Linq;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable.Powerups;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AchtungDieKurve.Game.Drawable
{
    public class PowerupsController : DefaultDrawable
    {
        private readonly IContainer _container;
        private readonly GridRegister _register;
        public Dictionary<string, Powerup> Available { get; private set; }
        public List<Powerup> OnScreen { get; private set; }
        private readonly Random _random;
        private readonly GameplayScreen _gameplayScreen;

        public PowerupsController(GameBase game, SpriteBatch spriteBatch, IContainer container,
            ref GridRegister register, GameplayScreen gameplayScreen)
            : base(game, spriteBatch)
        {
            _container = container;
            _register = register;
            _gameplayScreen = gameplayScreen;
            _random = new Random(GameBase.Settings.Rand.Next(int.MinValue, int.MaxValue));
        }

        public override void Update(GameTime gameTime)
        {
            if (!GameBase.Settings.PowerupsEnabled)
            {
                return;
            }

            base.Update(gameTime);
            if (_random.NextDouble() < GameBase.Settings.PowerupProbability)
            {
                var powerup = GetRandomPowerup();
                if (powerup != null)
                {
                    powerup.Postition = GetRandomCoordinates();
                    powerup.Ending += delegate(object sender, EventArgs args) { OnScreen.Remove(sender as Powerup); };
                    _register.Remember(powerup);
                    OnScreen.Add(powerup);
                }
            }

            foreach (var powerup in OnScreen)
            {
                powerup.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            foreach (var powerup in OnScreen)
            {
                powerup.Draw(gameTime);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            LoadPowerups();
        }

        private Vector2 GetRandomCoordinates()
        {
            return new Vector2(
                _random.Next(0, _container.ContentArea.Width - Powerup.Width),
                _random.Next(0, _container.ContentArea.Height - Powerup.Height)
                );
        }

        private Powerup GetRandomPowerup()
        {
            var threshold = _random.NextDouble();
            var passed = Register.Powerups.Where(info => info.Probability >= threshold).ToList();
            if (passed.Count == 0)
            {
                return null;
            }
            var idx = _random.Next(0, passed.Count);

            return Available[passed[idx].Name].Fork();
        }

        private void LoadPowerups()
        {
            Register.Load();
            Available = new Dictionary<string, Powerup>();
            OnScreen = new List<Powerup>();
            foreach (var info in Register.Powerups)
            {
                var powerup = CreatePowerupInstance(info.Name);
                powerup.LoadContent();
                Available.Add(info.Name, powerup);
            }
        }

        private Powerup CreatePowerupInstance(string className)
        {
            var type = Type.GetType("AchtungDieKurve.Game.Drawable.Powerups." + className);
            if (type == null)
            {
                throw new ArgumentException("Given Powerup name does not exists!");
            }
            var instance = Activator.CreateInstance(type, game, spriteBatch, _gameplayScreen);
            return (Powerup) instance;
        }

        public void Reset()
        {
            foreach (var powerup in OnScreen)
            {
                if (!powerup.HasBeenInvoked)
                {
                    continue;
                }
                powerup.Pause();
                powerup.Undo();
            }
            OnScreen.Clear();
        }
    }
}
