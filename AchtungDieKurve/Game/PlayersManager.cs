using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AchtungDieKurve.Game.AI;
using AchtungDieKurve.Game.Core;
using AchtungDieKurve.Game.Drawable;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace AchtungDieKurve.Game
{
    public delegate void WormEventTimed(Kurve k, GameTime gameTime);

    public delegate void WormEvent(Kurve k);

    public class PlayersManager
    {
        private const int InterRoundDelay = 2000;

        public event WormEvent Win;
        public event WormEvent PlayerAdded;
        public event EventHandler NextRound;

        private readonly GameBase _game;
        private int _living;

        public List<Kurve> Worms { get; private set; }

        public PlayersManager(GameBase game)
        {
            _game = game;
            Worms = new List<Kurve>();
            NextRound += StartNextRound;
        }


        public void AddPlayer(PlayerDefinition definition, IContainer container)
        {
            Kurve k = definition.IsAi
                ? new AiPlayer(GameBase.Settings, definition.Left, definition.Right, definition.Color, container)
                : new Player(GameBase.Settings, definition.Left, definition.Right, definition.Color, container);

            k.Death += PlayerDeath;
            k.Start += PlayerStart;

            Worms.Add(k);
            PlayerAdded?.Invoke(k);
        }

        private void PlayerStart(Kurve k)
        {
            _living++;
            GameBase.Settings.GameGoal += GameBase.Settings.GoalPlusPerPlayer;
        }

        public void AddPlayers(List<PlayerDefinition> list, IContainer container)
        {
            foreach (var info in list)
            {
                AddPlayer(info, container);
            }
        }


        private void PlayerDeath(Kurve k, GameTime gameTime)
        {
            GameBase.Log.Add($"Got death of player {k.Color.ToString()}");
            _game.SFX.PlayRandomCrash(gameTime, k);

            foreach (var player in Worms.Where(player => player != k && player.IsAlive))
            {
                player.Score++;
            }

            _living--;

            if (_living > 1) return;

            var winner = CheckForWinner();
            if (winner != null)
            {
                Win?.Invoke(winner);
            }
            else
            {
                NextRound?.Invoke(this, new EventArgs());
            }
        }

        private void StartNextRound(object sender, EventArgs e)
        {
            Thread.Sleep(InterRoundDelay);
            SetAndStart();
        }

        public void SetAndStart()
        {
            _living = 0;
            GameBase.Settings.GameGoal = 0;
            Reset();
        }

        public void Reset()
        {
            
        }

        private Kurve CheckForWinner() {

            var candidates = (
                from w in Worms
                where w.Score >= GameBase.Settings.GameGoal
                group w by w.Score into g
                orderby g.Key descending
                select g
                ).Take(1).FirstOrDefault();

            if (candidates == null) return null; // no one won yet
            if (candidates.Count() > 1) return null; // more people winning with tie 

            return candidates.First();
        }
            
   
        public static List<PlayerDefinition> GetPlayerDefinitions()
        {
            // load saved definition
            try
            {
                var saved = RegistrySettings.Read<List<PlayerDefinition>>("Players");
                if (saved == null)
                    return LoadDefaultPlayers();

                return saved;
            }
            catch
            {
                return LoadDefaultPlayers();
            }
        }

        private static List<PlayerDefinition> LoadDefaultPlayers()
        {
            var defaults = Properties.GetDefaultPlayerDefinitions();

            return defaults;
        }
    }
}
