using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    public class Fps : DefaultDrawable
    {
        private readonly IContainer _container;
        private const int RightPad = 10;
        private const int TopPad = 5;
        private TimeSpan _elapsedTime;
        private int _frameRate;
        private int _frameCounter;

        public Fps(GameBase game, SpriteBatch sb, IContainer container)
            : base(game, sb)
        {
            _container = container;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime <= TimeSpan.FromSeconds(1)) return;
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _frameCounter++;
            var fps = string.Format("FPS: {0}", _frameRate);
            var width = CommonResources.fontSmall.MeasureString(fps);
            var placement = new Vector2(
                _container.ContentArea.Width - RightPad - width.X,
                _container.ContentArea.Top + TopPad
                );
            Color color;
            if (_frameRate >= 40)
            {
                color = Color.Green;
            }
            else if (_frameRate >= 25)
            {
                color = Color.Yellow;
            }
            else
            {
                color = Color.Red;
            }
            spriteBatch.DrawString(CommonResources.fontSmall, fps, placement, color);
        }
    }
}
