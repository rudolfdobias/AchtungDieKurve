using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    public class Log : DefaultDrawable
    {
        private readonly IContainer _container;
        private const int RightPad = 10;
        private const int TopPad = 50;
        private List<string> content { get; set; } = new List<string>();

        public Log(GameBase game, SpriteBatch sb, IContainer container)
            : base(game, sb)
        {
            _container = container;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        public void Add(string x)
        {
            if (content.Count >= 30)
                content.RemoveAt(0);

            content.Add(x);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
          
            var placement = new Vector2(
                _container.ContentArea.Width - RightPad -250,
                _container.ContentArea.Top + TopPad
                );
            
            foreach (var x in content)
            {
                spriteBatch.DrawString(CommonResources.fontSmaller, x, placement, Color.Gray);
                placement.Y += 12;
            }
            
        }
    }
}
