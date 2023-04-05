using System;
using System.Timers;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    class Twister : Powerup
    {
        private readonly Timer _rotationTimer;
        private float _absolvedRotation;
        public Twister(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Twister(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
            _rotationTimer = TimerPool.CreateTimer();
        }

        public override PowerupType Type
        {
            get { return PowerupType.Neutral; }
        }

        public override int DurationMilliseconds
        {
            get { return 1; }
        }

        protected override string TextureName
        {
            get { return "twister"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            _rotationTimer.Interval = 1;
            _rotationTimer.Elapsed += RotationTimerOnElapsed;
            _rotationTimer.Start();
            
        }

        private void RotationTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var increment = (float)Math.PI/360;            ;
            Gameplay.Camera.Rotation += increment;
            Gameplay.Camera.HasModification = true;
            _absolvedRotation += increment;
            if (_absolvedRotation >= MathHelper.TwoPi)
            {
                _rotationTimer.Stop();
                Gameplay.Camera.Rotation = 0;
            }
        }


        public override void Undo()
        {
            Gameplay.Camera.HasModification = false;
            Gameplay.Camera.Rotation = 0;
        }
    }
}
