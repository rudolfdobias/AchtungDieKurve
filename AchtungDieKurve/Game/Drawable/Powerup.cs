using System;
using System.Timers;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    
    public enum PowerupType { ForMe, ForEnemy, Neutral }
    public abstract class Powerup : DefaultDrawable, ICollidable
    {
        public const int Width = 32;
        public const int Height = 32;
        public new bool Visible = true;
        private double _triggeredAt;
        public abstract PowerupType Type { get; }
        public abstract int DurationMilliseconds { get; }
        protected abstract string TextureName { get; }
        public abstract void Apply(Kurve entity, GameTime gameTime);
        public abstract void Undo();
        public event EventHandler Ending;
        public bool HasBeenInvoked;
        public event CollidableObjectMoved Move;
        protected GameplayScreen Gameplay;
        public Texture2D BodyTexture { get; set; }

        protected Timer ActionTimer;
        protected Kurve Invoker;

        public PotentialCollision CollisionBounds
        {
            get
            {
                return new PotentialCollision(this, new Rectangle((int)Postition.X, (int)Postition.Y, Width, Height), CollidableShape.Rectangle, CollisionCondition);
            }
        }

        public bool CollisionDisabled { get; private set; }
        public bool CanBeHit { get; private set; }

        public Vector2 Postition { get; set; }

        public Powerup(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay):base(game, spriteBatch)
        {
            Gameplay = gameplay; 
        }

        public Powerup(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch)
        {
            Gameplay = gameplay;
            BodyTexture = texture;
            CanBeHit = true;
            Visible = true;
            ActionTimer = TimerPool.CreateTimer();
        }

        public Powerup Fork()
        {
            return (Powerup)Activator.CreateInstance(GetType(), game, spriteBatch, Gameplay, BodyTexture);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            BodyTexture = game.Content.Load<Texture2D>("Powerups\\" + TextureName);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (!Visible)
            {
                return;
            }

            if (Postition == null)
            {
                throw new AccessViolationException("Cannot draw Powerup without position set.");
            }

            spriteBatch.Draw(BodyTexture, new Rectangle((int)Postition.X, (int)Postition.Y, Width, Height), Color.FromNonPremultiplied(255,255,255,127));
        }

   
        public void OnCollisionWith(ICollidable entity, GameTime gameTime)
        {
            throw new NotImplementedException("This should never happen");
        }

        public void WasHit(ICollidable entity, GameTime gameTime)
        {
            Visible = false;
            CanBeHit = false;
            var player = entity as Kurve;
            if (player == null)
            {
                return;
            }
            Invoker = player;
            Apply(player, gameTime);
            ActionTimer.Interval = DurationMilliseconds;
            ActionTimer.Elapsed += ActionTimerOnElapsed;
            ActionTimer.Start();
            _triggeredAt = gameTime.TotalGameTime.TotalMilliseconds;
            HasBeenInvoked = true;
        }

        private void ActionTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Undo();
            ActionTimer.Stop();
            if (Ending != null) { Ending(this, new EventArgs());}
        }

        public int RemainingTime(GameTime gameTime)
        {
            return (int)MathHelper.Clamp(DurationMilliseconds - (int)(gameTime.TotalGameTime.TotalMilliseconds - _triggeredAt), 0, DurationMilliseconds);
        }

        public int RemainingPercent(GameTime gameTime)
        {
            var remaining = RemainingTime(gameTime);
            if (remaining <= 0)
            {
                return 0;
            }
            return (int)MathHelper.Clamp(remaining / (DurationMilliseconds / 100), 0, 100);
        }


        public bool CollisionCondition(ICollidable me, PotentialCollision company, GameTime gameTime)
        {
            return CanBeHit;
        }

        public void Pause()
        {
            ActionTimer.Stop();
        }

        public void UnPause()
        {
            ActionTimer.Start();
        }
    }
}
