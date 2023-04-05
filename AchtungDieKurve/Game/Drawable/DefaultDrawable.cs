using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game.Drawable
{
    public abstract class DefaultDrawable : IDrawable, IUpdateable
    {
        protected bool _visible = true;
        protected bool _enabled = true;
        protected int _updateOrder = 1;
        protected int _drawOrder = 1;

        protected SpriteBatch spriteBatch;
        protected GameBase game;

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void HandleInput(InputState state) { }
        public virtual void LoadContent() { }

        protected virtual void Init() { }

        public DefaultDrawable() {
            Init();
        }

        public DefaultDrawable(GameBase game, SpriteBatch spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;

            Init();
        }
        
        public int UpdateOrder
        {
            get { throw new NotImplementedException(); }
        }

        public bool Enabled
        {
            get { return this._enabled; }
        }

        public int DrawOrder
        {
            get { return this._drawOrder; }
        }

        public bool Visible
        {
            get { return this._visible; }
        }

        
    }
}
