using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class SlowEnemy : Powerup
    {
        public override PowerupType Type { get { return PowerupType.ForEnemy;} }
        public const float SpeedAddition = 0.9f;
        private readonly Dictionary<int,float> _lastSpeeds;

        public SlowEnemy(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public SlowEnemy(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
            _lastSpeeds = new Dictionary<int, float>();
        }

        public override int DurationMilliseconds
        {
            get { return 6000; }
        }

        protected override string TextureName
        {
            get { return "slow_red"; }
        }

        public override void Apply(Kurve entity, GameTime gameTime)
        {
            for (var i = 0; i < Gameplay.PlayersManager.Worms.Count; i++)
            {
                var p = Gameplay.PlayersManager.Worms[i];
                if (p == Invoker)
                {
                    continue;
                }
                var delta = p.Speed - SpeedAddition;
                p.Speed -= SpeedAddition;
                if (delta < p.Speed)
                {
                    _lastSpeeds[i] = SpeedAddition - (p.Speed - delta);
                }
                else
                {
                    _lastSpeeds[i] = SpeedAddition;
                }
                p.ActivePowerups.Add(this);
            }
        }

        public override void Undo()
        {
            for (var i = 0; i < Gameplay.PlayersManager.Worms.Count; i++)
            {
                var p = Gameplay.PlayersManager.Worms[i];
                if (p == Invoker)
                {
                    continue;
                }
                p.Speed += _lastSpeeds[i];
                p.ActivePowerups.Remove(this);
            }
        }
    }
}
