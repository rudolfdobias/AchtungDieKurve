using System;
using AchtungDieKurve.Game.AI.Activities;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.AI
{
    public class AiDriver
    {
        private readonly GridRegister _register;
        private IContainer _container;
        public AiDriver(IContainer container, ref GridRegister register){
            _container = container;
            _register = register;
        }

        public void ControlAi(Kurve player, KeyboardState keyboardState, GameTime gameTime)
        {
            if (player.CollisionBounds == null) { return; }
            var neighborhood = _register.Neighborhood(player, gameTime, player.CollisionCondition, 64);
            if (neighborhood.Count == 0) { return; }
            var arcMatrix = new ArcMatrix(player as AiPlayer);
            IAiActivity[] activities = {new Survive()};
            var desiredAngle = arcMatrix.Load(neighborhood).Decide(activities).ProposeAngle();
            ControlPlayer(player as AiPlayer, desiredAngle);
        }

        public void DrawDebug(SpriteBatch sb, AiPlayer player, GameTime gameTime)
        {
            if (player == null || player.CollisionBounds == null) { return; }
            var neighborhood = _register.Neighborhood(player, gameTime, player.CollisionCondition, 64);
            if (neighborhood.Count == 0) { return; }
            var arcMatrix = new ArcMatrix(player);
            IAiActivity[] activities = { new Survive() };
            arcMatrix.DrawDebug(sb, neighborhood, activities);
        }

        public void ControlPlayer(AiPlayer player, double desiredAngle)
        {
            int direction;
            if (Math.Abs(player.Angle - desiredAngle) < 0.0001f)
            {
                return;
            }
            var diff = player.Angle - desiredAngle;
            if (diff > 0) { direction = -1; }
            else { direction = 1; }
            if (Math.Abs(diff) > MathHelper.TwoPi) { direction = direction * -1; }
            switch (direction)
            {
                case 1:
                    player.TurnRight();
                    break;
                case -1:
                    player.TurnLeft();
                    break;
                default:
                    if (player.Context.Rand.NextDouble() > 0.5)
                    { player.TurnLeft(); }
                    else { player.TurnRight(); }
                    break;
            }
        }

        
    }
}