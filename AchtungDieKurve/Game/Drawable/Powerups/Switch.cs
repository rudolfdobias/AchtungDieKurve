using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    internal struct PlayerDirections
    {
        public Keys Right { get; set; }

        public Keys Left { get; set; }

        public PlayerDirections(Keys left, Keys right) : this()
        {
            Left = left;
            Right = right;
        }
    }

    public class Switch : Powerup
    {
        public override PowerupType Type { get { return PowerupType.ForEnemy;} }

        private Dictionary<int,PlayerDirections> _reverse;

        public Switch(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public Switch(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
            _reverse = new Dictionary<int, PlayerDirections>();
        }

        public override int DurationMilliseconds
        {
            get { return 8000; }
        }

        protected override string TextureName
        {
            get { return "switch_red"; }
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
                _reverse[i] = new PlayerDirections(p.Left, p.Right);
                var temp = p.Right;
                p.Right = p.Left;
                p.Left = temp;
                
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
                p.Left = _reverse[i].Left;
                p.Right = _reverse[i].Right;
                p.ActivePowerups.Remove(this);
            }
        }
    }
}
