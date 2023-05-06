using System;
using System.Collections.Generic;
using System.Linq;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Core
{
    public class GridRegister
    {
        private const int DefaultScanRadius = 32;
        private readonly int _gridRaster;
        private readonly List<PotentialCollision>[,] _gridRegister;
        private readonly SpriteBatch _sb;
        private readonly IContainer _container;
        private float _ratioX;
        private float _ratioY;
        private int _cellSize;


        public int Raster { get { return _gridRaster; } }

        public GridRegister(SpriteBatch sb, IContainer container, int rasterSize)
        {
            _gridRaster = rasterSize;
            _container = container;
            _sb = sb;
            _gridRegister = new List<PotentialCollision>[_gridRaster + 1, _gridRaster + 1];
            CreateGrid();
            Reset();
        }

        public void Remember(ICollidable entity)
        {
            var centerCell = GetGridCenterCell(entity);
            if (_cellSize < (entity.CollisionBounds.Bounds.Width/2))
            {
                // iterate X
                for (var x = (int) coordinatesToGrid(entity.CollisionBounds.Origin).X + 1;
                    x < coordinatesToGrid(entity.CollisionBounds.Bounds.Width, 0).X;
                    x++)
                {
                    _gridRegister[x, (int) centerCell.Y].Add(
                        entity.CollisionBounds.CreateSegment(gridToCoordinates(x, (int) centerCell.Y)));
                }
            }
            if (_cellSize < (entity.CollisionBounds.Bounds.Height/2))
            {
                // iterate Y
                for (var y = (int) coordinatesToGrid(entity.CollisionBounds.Origin).Y + 1;
                    y < coordinatesToGrid(0, entity.CollisionBounds.Bounds.Height).Y;
                    y++)
                {
                    _gridRegister[(int) centerCell.X, y].Add(
                        entity.CollisionBounds.CreateSegment(gridToCoordinates((int) centerCell.X, y)));
                }
            }

            _gridRegister[(int) centerCell.X, (int) centerCell.Y].Add(entity.CollisionBounds);
        }

        public void Remember(List<ICollidable> list)
        {
            foreach (var block in list) { Remember(block); }
        }

        public bool Find(ICollidable entity, GameTime gameTime, CollisionDetection action, int scanRadius = DefaultScanRadius)
        {
            return GetCriticalRadius(GetGridCenterCell(entity), scanRadius)
                .SelectMany(field => _gridRegister[(int) field.X, (int) field.Y])
                .Any(company => action(entity, company, gameTime));
        }

        public List<PotentialCollision> Neighborhood(ICollidable entity, GameTime gameTime, CollisionDetection collisionCondition, int scanRadius = DefaultScanRadius)
        {
            return (from field in GetCriticalRadius(GetGridCenterCell(entity), scanRadius)
                from company in _gridRegister[(int) field.X, (int) field.Y]
                where collisionCondition(entity, company, gameTime)
                select company).ToList();
        }

        public Vector2 GetGridCenterCell(ICollidable entity)
        {
            return coordinatesToGrid(entity.CollisionBounds.Center);
        }

        public List<Vector2> GetCriticalRadius(Vector2 center, int radius)
        {
            var circle = new List<Vector2>();
            var localRadius = (int)Math.Ceiling(radius / (float)_cellSize);
            var crop = new Vector4(
                MathHelper.Clamp(center.X - localRadius, 0, _ratioX),
                MathHelper.Clamp(center.Y - localRadius, 0, _ratioY),
                MathHelper.Clamp(center.X + localRadius, 0, _ratioX),
                MathHelper.Clamp(center.Y + localRadius, 0, _ratioY)
                );

            for (var x = (int)crop.X; x <= crop.Z; x++)
            {
                for (var y = (int)crop.Y; y <= crop.W; y++)
                {
                    var actual = new Vector2(x, y);
                    if (Vector2.Distance(
                        actual,
                        new Vector2(center.X, center.Y)
                        ) <= localRadius) { circle.Add(actual); }
                }
            }

            return circle;
        }

        public void Draw(GameTime gameTime)
        {

            if (GameBase.Defaults.DebugCollisions == false)
                return;
            // draw grid
            for (int x = 1; x <= _ratioX; x++)
            {
                Vector2 coords = gridToCoordinates(x, 0);
                _sb.Draw(CommonResources.whitepixel, 
                    (new Rectangle((int)coords.X, 0, 1, _container.ContentArea.Height)),
                    Color.FromNonPremultiplied(10, 10, 10, 255));
            }
            for (int y = 1; y <= _ratioY; y++)
            {
                Vector2 coords = gridToCoordinates(0, y);
                _sb.Draw(CommonResources.whitepixel,
                    (new Rectangle(0, (int)coords.Y, _container.ContentArea.Width, 1)),
                    Color.FromNonPremultiplied(10, 10, 10, 255));
            }

            for (int x = 0; x <= _ratioX; x++){
                for (int y = 0; y <= _ratioY; y++)
                {
                    Vector2 coords = gridToCoordinates(x, y);
                    int alpha = _gridRegister[x, y].Count;
                    _sb.Draw(CommonResources.whitepixel,
                        new Rectangle(
                            (int)coords.X,
                            (int)coords.Y,
                            _cellSize,
                            _cellSize
                        ),
                        Color.FromNonPremultiplied(40, 200, 0, alpha)
                        );
                }
            }

        }

        public void DrawRadius(ICollidable entity)
        {
            var radius = GetCriticalRadius(GetGridCenterCell(entity), 64);
            foreach (var coords in radius.Select(gridToCoordinates))
            {
                _sb.Draw(CommonResources.whitepixel, 
                    new Rectangle(
                        (int)(coords.X), 
                        (int)(coords.Y), 
                        _cellSize, _cellSize), 
                    Color.FromNonPremultiplied(255, 255, 255, 20));
            }
        }

        private void CreateGrid()
        {
            _cellSize = (int)Math.Ceiling(_container.ContentArea.Width / (float)Raster);
            _ratioX = Raster;
            _ratioY = (float)Math.Ceiling(_container.ContentArea.Height / (float)_container.ContentArea.Width * _ratioX);         
        }

        private void CleanGrid()
        {
            for (var x = 0; x < _ratioX + 1; x++)
            {
                for (var y = 0; y < _gridRaster + 1; y++)
                {
                    _gridRegister[x, y] = new List<PotentialCollision>();
                }
            }
        }

        public Vector2 coordinatesToGrid(float x, float y)
        {
            return new Vector2(
                _ratioX * x / _container.ContentArea.Width,
                _ratioY * y / _container.ContentArea.Height
                );           
        }

        public Vector2 coordinatesToGrid(Vector2 reals)
        {
            return coordinatesToGrid(reals.X, reals.Y);
        }

        public Vector2 RectangleOriginToGrid(Rectangle rectangle)
        {
            return coordinatesToGrid(rectangle.X, rectangle.Y);
        }

        public Vector2 RectangleCenterToGrid(Rectangle rectangle)
        {
            return coordinatesToGrid(rectangle.Center.X, rectangle.Center.Y);
        }

        public Vector2 gridToCoordinates(int x, int y)
        {
            return new Vector2(
                _container.ContentArea.Width / _ratioX * x,
                _container.ContentArea.Height / _ratioY * y
                );
        }

        public Vector2 gridToCoordinates(Vector2 gridCoords)
        {
            return gridToCoordinates((int)gridCoords.X, (int)gridCoords.Y);
        }

        public void Reset()
        {
            CleanGrid();
        }
    }
}