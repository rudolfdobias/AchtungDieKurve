using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AchtungDieKurve.Game.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.Drawable
{
    public struct PowerupProgressBar
    {
        public PowerupProgressBar(Rectangle background, Rectangle foreground, Color fgColor, Color bgColor) : this()
        {
            Background = background;
            Foreground = foreground;
            BgColor = bgColor;
            FgColor = fgColor;
        }

        public Rectangle Background { get; set; }

        public Rectangle Foreground { get; set; }

        public Color BgColor { get; set; }

        public Color FgColor { get; set; }
    }

    public abstract class Kurve : ICollidable
    {
        // NEW SHIT

        public event WormEvent Start;
        public event WormEventTimed Death;
        public event WormEvent ScoreChanged;
        public event WormEvent EndProtection;
        public event WormEvent StartDrawingHole;
        public event WormEvent StopDrawingHole;
        public event CollidableObjectMoved Move;
        public event CollidableObjectMoved Drawed;

        protected IContainer Container;
        public Keys Left;
        public Keys Right;
        public Color Color;

        public bool WallTraversalEnabled { get; set; }
        
        public float TurnStep { get; set; }
        public Vector2 AbsolutePosition;
        public Rectangle ActualBounds;
        public Texture2D BodyTexture { get; set; }
        protected Timer ProtectionTimer;
        private Random _randomGenerator;
        public List<Rectangle> Body = new List<Rectangle>();
        public List<Powerup> ActivePowerups = new List<Powerup>(); 
        public bool CollisionDisabled
        {
            get { return !_drawingEnabled || _hasProtection; }
        }

        public bool CanBeHit { get; private set; }

        public PotentialCollision CollisionBounds
        {
            get
            {
                switch (Body.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return new PotentialCollision(this, Body.Last(), CollidableShape.Rectangle, CollisionCondition);
                }
                return new PotentialCollision(this, Body[Body.Count - 1], CollidableShape.Rectangle,
                    CollisionCondition);
            }
        }

        private int _score;
        private float _angle;
        private int _diameter = 8;
        protected bool _isAlive = true;
        public bool _hasProtection;
        protected bool _drawingHole;
        protected bool _ai;
        protected bool _drawingEnabled;
        
        public int Score {
            get { return _score; }
            set { 
                _score = value;
                ScoreChanged?.Invoke(this);
            }
        }

        public float Angle
        {
            get { return _angle; }
            set { _angle = MathHelper.WrapAngle(value); }
        }

        public int Diameter
        {
            get { return _diameter; }
            set { _diameter = (int)MathHelper.Clamp(value, 1.0f, GameBase.Settings.MaxPlayerThickness); }
        }

        public int Radius
        {
            get { return (int)MathHelper.Clamp(value: (float)Diameter / 2, min: 1.0f, max: GameBase.Settings.MaxPlayerThickness / 2); }
        }

        public float Speed
        {
            get { return _speed; }
            set
            {
                _speed = MathHelper.Clamp(value, GameBase.Settings.MinPlayerSpeed, GameBase.Settings.MaxPlayerSpeed);
            }
        }

        public bool HasProtection
        {
            get { return _hasProtection; }
        }

        public bool DrawingEnabled
        {
            get { return _drawingEnabled; }
        }
        public bool IsAlive
        {
            get { return _isAlive; }
        }

        public bool IsAi
        {
            get { return _ai; }
        }

        public Properties Context;
        private float _speed;

        protected Kurve()
        {
            EndProtection += Kurve_EndProtection;
            Start += Kurve_Start;
            StartDrawingHole += Kurve_StartDrawingHole;
            StopDrawingHole += Kurve_StopDrawingHole;
            var time = DateTime.UtcNow;
            _randomGenerator = new Random(time.TimeOfDay.Milliseconds);
        }


        public void LoadContent(ContentManager manager)
        {
            BodyTexture = manager.Load<Texture2D>("body");
        }
       
        public void Turn(KeyboardState ks)
        {
            if (ks.IsKeyDown(Left))
            {
                Angle -= TurnStep;
            }
            else if (ks.IsKeyDown(Right))
            {
                Angle += TurnStep;
            }
            
        }

        public void SetContext(Properties properties)
        {
            Context = properties;
        }

        public void Draw(GameTime gameTime, SpriteBatch sb)
        {        
            // Draw actual position
            sb.Draw(BodyTexture, ActualBounds, Color);

            // Draw the rest of former positions - body
            foreach (var r in Body)
            {
                sb.Draw(BodyTexture, r, Color);
            }

            if (DrawingEnabled)
            {
                Body.Add(ActualBounds);
            }

            if (Drawed != null) { 
                Drawed(this, gameTime); }
        }

        public void Update(KeyboardState keyboardState, GameTime gameTime)
        {
            if (!_isAlive) { return; }

            Control(keyboardState, gameTime);
            
            ComputeActualBounds();
            CreateHoles();

            if (BoundaryCollision())
            {
                if (WallTraversalEnabled)
                {
                    TraverseWall();
                }
                else if (_hasProtection && GameBase.Settings.WallBounceWhileProtection)
                {
                    WallBounce();
                }
            }

            Move?.Invoke(this, gameTime);
        }

        protected virtual void Control(KeyboardState keyboardState, GameTime gameTime)
        {
            Turn(keyboardState);
        }

        private void CreateHoles()
        {
            if (_drawingHole)
            {
                if (_randomGenerator.NextDouble() <= GameBase.Settings.HoleTerminationProbability && StopDrawingHole != null) { StopDrawingHole(this); }
            }
            else if (_drawingEnabled && !_hasProtection)
            {
                if (_randomGenerator.NextDouble() <= GameBase.Settings.HoleProbability && StartDrawingHole != null) { StartDrawingHole(this); }
            }
        }

        private void TraverseWall()
        {
            
            if (ActualBounds.Left <= 0)
                AbsolutePosition.X = Container.ContentArea.Width - (Radius +3);
            else if (ActualBounds.Right >= Container.ContentArea.Width)
                AbsolutePosition.X = Radius + 3;

            if (ActualBounds.Top <= 0)
                AbsolutePosition.Y = Container.ContentArea.Height - (Radius +3);
            else if (ActualBounds.Bottom >= Container.ContentArea.Height)
                AbsolutePosition.Y = Radius + 3;
            
            /*
            AbsolutePosition = new Vector2(
                MathHelper.Clamp(AbsolutePosition.X, Radius +1, Container.ContentArea.Width - Radius -1),
                MathHelper.Clamp(AbsolutePosition.Y, Radius +1, Container.ContentArea.Height - Radius -1)
            );*/

            //Angle += (float)Math.PI;
        }

        private void WallBounce()
        {
            AbsolutePosition = new Vector2(
                MathHelper.Clamp(AbsolutePosition.X, Radius +1, Container.ContentArea.Width - Radius -1),
                MathHelper.Clamp(AbsolutePosition.Y, Radius +1, Container.ContentArea.Height - Radius -1)
                );
            var v = new Vector2(Angle);

            // Bounce angle equals hit angle...
            if (ActualBounds.Left <= 0 || ActualBounds.Right >= Container.ContentArea.Width)
            {
                v.X = -v.X;
            }
            else
            {
                v.Y = -v.Y;
            }
            Angle = v.Length();
        }

        private void ComputeActualBounds()
        {
            AbsolutePosition += new Vector2(
                (float)(Speed * Math.Cos(Angle)),
                (float)(Speed * Math.Sin(Angle))
                );
            ActualBounds = new Rectangle(
                (int)MathHelper.Clamp(AbsolutePosition.X - Radius, 0, Container.ContentArea.Width),
                (int)MathHelper.Clamp(AbsolutePosition.Y - Radius, 0, Container.ContentArea.Height), 
                Diameter, Diameter);
        }

        public bool CollisionCondition(ICollidable me, PotentialCollision company, GameTime gameTime)
        {
            if (company.Owner != me)
            {
                return true;
            }
            
            // Compute actual velocity vector - length delta between this square and next square origins
            var velocity =  Math.Abs(new Vector2(
                (float)(Speed * Math.Cos(Angle)),
                (float)(Speed * Math.Sin(Angle))
                ).Length());
            var bodyLength = Body.Count();
            if (bodyLength == 0) { return false; }
            var safeRange = bodyLength;
            if (velocity > 0){
                safeRange = (int)Math.Ceiling(Diameter * Math.PI) +1;
            } 
            safeRange = (int)MathHelper.Clamp(safeRange, 0, bodyLength);
            return !Body.GetRange(bodyLength - safeRange, safeRange).Contains(company.Bounds);
        }

        private bool BoundaryCollision()
        {
            var hitTheWall = (
                ActualBounds.Left <= 0 ||
                ActualBounds.Right >= Container.ContentArea.Width ||
                ActualBounds.Top <= 0 ||
                ActualBounds.Bottom >= Container.ContentArea.Height
                );
            return hitTheWall;
        }

        public void StartProtection(int milliseconds)
        {
            ProtectionTimer = TimerPool.CreateTimer();
            ProtectionTimer.Interval = milliseconds;
            ProtectionTimer.Elapsed += protectionTimer_Elapsed;
            ProtectionTimer.AutoReset = false;
            ProtectionTimer.Start();

            _hasProtection = true;
            _drawingEnabled = false;
        }

        public void TriggerEndProtection()
        {
            EndProtection?.Invoke(this);
        }

        
        void protectionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            EndProtection?.Invoke(this);
        }

        void Kurve_StopDrawingHole(Kurve k)
        {
            _drawingEnabled = true;
            _drawingHole = false;
        }

        void Kurve_StartDrawingHole(Kurve k)
        {
            _drawingEnabled = false;
            _drawingHole = true;
        }

        void Kurve_Start(Kurve k)
        {
            StartProtection(GameBase.Settings.InitialProtectionTime);
        }

        void Kurve_EndProtection(Kurve k)
        {
            _drawingEnabled = true;
            _hasProtection = false;
        }

        public void Kill(GameTime gameTime)
        {
            _isAlive = false;
            Death?.Invoke(this, gameTime);
        }

        public void Live()
        {
            _isAlive = true;
            Start?.Invoke(this);
        }

        // -------------------------- ICollidable implementation ----

        public PotentialCollision GetActualBounds()
        {
            return new PotentialCollision(this, ActualBounds, CollidableShape.Rectangle, CollisionCondition);
        }

    
        public void OnCollisionWith(ICollidable entity, GameTime gameTime)
        {
            if (IsAlive == false)
                return;

            if (entity is Kurve || entity is Wall)
            {
                _isAlive = false;
                Death?.Invoke(this, gameTime);
            } 
        }

        public void WasHit(ICollidable entity, GameTime gameTime)
        {
            
        }

        public void DrawPowerupStats(SpriteBatch sb, GameTime gameTime)
        {
            var barContainer = new Rectangle(ActualBounds.X + 16, ActualBounds.Y + 16, 32, 64);
            for (var idx = 0; idx < ActivePowerups.Count; idx++)
            {
                var p = ActivePowerups[idx];
                var bar = CreatePowerupBar(p, barContainer, idx, gameTime);
                sb.Draw(CommonResources.whitepixel, bar.Background, bar.BgColor);
                sb.Draw(CommonResources.whitepixel, bar.Foreground, bar.FgColor);
                //sb.DrawString(CommonResources.fontSmaller, p.RemainingPercent(gameTime).ToString(), new Vector2(barContainer.X, barContainer.Y), Color.White);
            }
        }

        private PowerupProgressBar CreatePowerupBar(Powerup p, Rectangle container, int idx, GameTime gameTime)
        {
            return new PowerupProgressBar(
                new Rectangle(container.X, container.Y + (idx*4), container.Width, 3),
                new Rectangle(container.X, container.Y + (idx*4),
                    (int) (container.Width*((double)p.RemainingPercent(gameTime)/100)), 3),
                p.Type == PowerupType.ForEnemy ? Color.Red : Color.Green,
                Color.FromNonPremultiplied(20, 20, 20, 200));
        }

        //reset fields for next round
        public virtual void Reset()
        {
            Body.Clear();
            var x = Context.Rand.Next((int)(Container.ContentArea.Width * 0.4)) + (int)(Container.ContentArea.Width * 0.2);
            var y = Context.Rand.Next((int)(Container.ContentArea.Height * 0.4)) + (int)(Container.ContentArea.Height * 0.2);
            AbsolutePosition = new Vector2(x, y);
            Angle = MathHelper.WrapAngle((float)Context.Rand.NextDouble() * 2.0f * (float)Math.PI);
            _hasProtection = true;
            _drawingEnabled = false;
            _drawingHole = false;
            _randomGenerator = new Random((int)Angle);
            Diameter = GameBase.Settings.DefaultDiameter;
            TurnStep = GameBase.Settings.DefaultTurnStep;
            Speed = GameBase.Settings.DefaultSpeed;
        }


        
    }
}