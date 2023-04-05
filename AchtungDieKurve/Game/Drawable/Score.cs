using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable
{
    public class Score : DefaultDrawable, IContainer
    {
        private const int MarginLeft = 5;
        private const int MarginTop = 9;
        private const int MarginRight = 0;
        private const int MarginBottom = 0;
        private const int ScoreCellSpacing = 10;
        private const int ScoreCellWidth = 45;
        private const int ScoreCellHeight = 45;

        private readonly IContainer _container;
        private readonly List<Kurve> _players;

        private readonly Color _goalBarColor;
        private readonly Color _goalTextColor;

        public Score(IContainer container, List<Kurve> players, GameBase game, SpriteBatch spriteBatch)
            : base(game, spriteBatch)
        {
            _container = container;
            _players = players;

            _goalBarColor = Color.FromNonPremultiplied(70, 70, 70, 255);
            _goalTextColor = Color.White;
        }

        public Rectangle ContentArea
        {
            get
            {
                return new Rectangle(_container.ContentArea.Left + MarginLeft, _container.ContentArea.Top + MarginTop,
                    _container.ContentArea.Right - MarginRight, _container.ContentArea.Bottom - MarginBottom);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var placement = new Rectangle
            {
                X = ContentArea.X,
                Y = ContentArea.Y,
                Width = ScoreCellWidth,
                Height = ScoreCellHeight
            };
            foreach (var kurva in _players)
            {
                DrawPlayerScore(kurva, placement);
                placement.Y += ScoreCellSpacing + ScoreCellHeight;
            }

            DrawGoal(placement);
        }

        private void DrawPlayerScore(Kurve kurva, Rectangle placement)
        {
            spriteBatch.Draw(CommonResources.whitepixel, placement, kurva.Color);
            // black subRectangle covering the placement, creates a border effect
            spriteBatch.Draw(CommonResources.whitepixel, new Rectangle(placement.X + 3, placement.Y + 3, 42, 42),
                CommonResources.Background);
            var scorePlacement = Helper.CenterText(CommonResources.fontSmaller, kurva.Score.ToString(), placement);
            spriteBatch.DrawString(
                CommonResources.fontSmaller,
                kurva.Score.ToString(),
                scorePlacement,
                Color.White);
        }

        private void DrawGoal(Rectangle placement)
        {
            var goalText = GameBase.Settings.GameGoal.ToString();
            var divisionBar = new Rectangle(placement.X, placement.Y, placement.Width, 1);
            spriteBatch.Draw(CommonResources.whitepixel, divisionBar, _goalBarColor);
            var textPlacement = Helper.CenterText(CommonResources.fontSmaller, goalText, placement);
            spriteBatch.DrawString(
                CommonResources.fontSmaller,
                goalText,
                textPlacement,
                _goalTextColor);
        }
    }
}
