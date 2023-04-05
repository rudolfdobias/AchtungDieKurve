using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Parts.Menu
{
    class EditNameBox : DefaultDrawable
    {
        private readonly PlayerDefinition _definition;

        public EditNameBox(GameBase game, SpriteBatch spriteBatch, PlayerDefinition definition)
            : base(game, spriteBatch)
        {
            _definition = definition;
        }

    }
}
