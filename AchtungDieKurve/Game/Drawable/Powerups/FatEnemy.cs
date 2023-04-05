using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AchtungDieKurve.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public class FatEnemy : Powerup
    {
        public override PowerupType Type { get { return PowerupType.ForEnemy;} }
        public const int Thickness = 5;
        private Dictionary<int,int> _reverse;

        public FatEnemy(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay)
            : base(game, spriteBatch, gameplay)
        {
        }

        public FatEnemy(GameBase game, SpriteBatch spriteBatch, GameplayScreen gameplay, Texture2D texture)
            : base(game, spriteBatch, gameplay, texture)
        {
            _reverse = new Dictionary<int, int>();
        }

        public override int DurationMilliseconds
        {
            get { return 8000; }
        }

        protected override string TextureName
        {
            get { return "fat_red"; }
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
                var delta = p.Diameter + Thickness;
                p.Diameter += Thickness;
                if (delta > p.Diameter)
                {
                    _reverse[i] = Thickness - (delta - p.Diameter);
                }
                else
                {
                    _reverse[i] = Thickness;
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
                p.Diameter -= _reverse[i];
                p.ActivePowerups.Remove(this);
            }
        }
    }
}
