

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Graphics
{
    public class Camera2D
    {
        #region Fields

        protected float _zoom;
        protected Matrix _transform;
        protected Matrix _inverseTransform;
        protected Vector2 _pos;
        protected float _rotation;
        protected Viewport _viewport;
        protected MouseState _mState;
        protected KeyboardState _keyState;
        protected Int32 _scroll;
        protected bool _locked = false;
        public bool HasModification;

        #endregion

        #region Properties

        public bool Locked
        {
            get { return _locked; }
            set { _locked = value; }
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        /// <summary>
        /// Camera View Matrix Property
        /// </summary>
        public Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        /// <summary>
        /// Inverse of the view matrix, can be used to get objects screen coordinates
        /// from its object coordinates
        /// </summary>
        public Matrix InverseTransform
        {
            get { return _inverseTransform; }
        }

        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        #endregion

        #region Constructor

        public Camera2D()
        {
            Viewport viewport = GameBase.Graphics.GraphicsDevice.Viewport;
            _zoom = 1.0f;
            _scroll = Mouse.GetState().ScrollWheelValue;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
            _viewport = viewport;
        }

        public Matrix GetSpecialCamera()
        {
            if (!HasModification)
            {
                return Transform;
            }

            Matrix translateToOrigin = Matrix.CreateTranslation(-GameBase.Settings.ScreenWidth/2,
                -GameBase.Settings.ScreenHeight/2, 0);
            Matrix translateBackToPosition = Matrix.CreateTranslation(GameBase.Settings.ScreenWidth/2,
                GameBase.Settings.ScreenHeight/2/2, 0);
            //Create view matrix
            var _newTransform =
                 translateToOrigin*
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                Matrix.CreateTranslation(_pos.X, _pos.Y, 0) *
            translateBackToPosition;

            return _newTransform;
            //Update inverse matrix
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the camera view
        /// </summary>
        public void Update()
        {
            //Call Camera Input
            HandleInput();
            //Clamp zoom value
            _zoom = MathHelper.Clamp(_zoom, 0.0f, 10.0f);
            //Clamp rotation value
            _rotation = ClampAngle(_rotation);

            /*Matrix translateToOrigin = Matrix.CreateTranslation(-GameBase.Settings.ScreenWidth/2,
                -GameBase.Settings.ScreenHeight/2, 0);
            Matrix translateBackToPosition = Matrix.CreateTranslation(GameBase.Settings.ScreenWidth/2,
                GameBase.Settings.ScreenHeight/2/2, 0);*/
            //Create view matrix
            _transform =
                // translateToOrigin*
                Matrix.CreateRotationZ(_rotation)*
                Matrix.CreateScale(new Vector3(_zoom, _zoom, 1))*
                Matrix.CreateTranslation(_pos.X, _pos.Y, 0);//*
                //translateBackToPosition;
            //Update inverse matrix
            _inverseTransform = Matrix.Invert(_transform);
        }

        /// <summary>
        /// Example Input Method, rotates using cursor keys and zooms using mouse wheel
        /// </summary>
        protected virtual void HandleInput()
        {
            if (_locked)
            {
                return;
            }

            _mState = Mouse.GetState();
            _keyState = Keyboard.GetState();
            //Check zoom
            if (_mState.ScrollWheelValue > _scroll)
            {
                _zoom += 0.2f;
                _scroll = _mState.ScrollWheelValue;
            }
            else if (_mState.ScrollWheelValue < _scroll)
            {
                _zoom -= 0.2f;
                _scroll = _mState.ScrollWheelValue;
            }
            //Check rotation
            if (_keyState.IsKeyDown(Keys.Left))
            {
                _rotation -= 0.1f;
            }
            if (_keyState.IsKeyDown(Keys.Right))
            {
                _rotation += 0.1f;
            }
            //Check Move
            if (_keyState.IsKeyDown(Keys.A))
            {
                _pos.X += 2.5f;
            }
            if (_keyState.IsKeyDown(Keys.D))
            {
                _pos.X -= 2.5f;
            }
            if (_keyState.IsKeyDown(Keys.W))
            {
                _pos.Y += 2.5f;
            }
            if (_keyState.IsKeyDown(Keys.S))
            {
                _pos.Y -= 2.5f;
            }
        }

        /// <summary>
        /// Clamps a radian value between -pi and pi
        /// </summary>
        /// <param name="radians">angle to be clamped</param>
        /// <returns>clamped angle</returns>
        protected float ClampAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        #endregion
    }
}
