using System;
using System.Collections.Generic;
using AchtungDieKurve.Game;
using AchtungDieKurve.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AchtungDieKurve
{
    public class Properties
    {
        // Core ------------------------------------------------------------- 
        public bool DebugCoordinates = false;
        public bool DebugCollisions = false;

        // Game configuration -----------------------------------------------     
        public int GoalPlusPerPlayer = 5;
        public int InitialProtectionTime = 2000;
        public bool PowerupsEnabled = true;

        // View -- set by GraphicsManager on every display mode change -------
        public int ScreenWidth;
        public int ScreenHeight;

        // Player curve specs -----------------------------------------------
        public int MaxPlayerThickness = 100;
        public bool WallBounceWhileProtection = true;
        public double HoleProbability = 0.009;
        public double HoleTerminationProbability = 0.1;
        public float DefaultSpeed = 2.2f;
        public float DefaultTurnStep = 0.05f;
        public int DefaultDiameter = 8;
        public float MinPlayerSpeed = 0.5f;
        public float MaxPlayerSpeed = 20f;

        // Powerups ---------------------------------------------------------
        public double PowerupProbability = 0.007;

        // AI: 0..1. Aggressiveness = how often the AI attacks instead of just
        // surviving; Precision = reaction speed and steering accuracy.
        public float AiAggressiveness = 0.55f;
        public float AiPrecision = 0.95f;

        // Runtime vars -----------------------------------------------------
        public int GameGoal = 0;


        // OLD SHIT
        public Random Rand = new();
        public SfxController Sfx;



        // Default player definition ----------------------------------------

        public static List<PlayerDefinition> GetDefaultPlayerDefinitions()
        {
            
            return new List<PlayerDefinition>
            {
                new PlayerDefinition("The Red One", Keys.Left, Keys.Right, Color.Red),
                new PlayerDefinition("Blue Guy", Keys.X, Keys.C, Color.Blue),
                new PlayerDefinition("Green Bastard", Keys.Q, Keys.A, Color.Green),
                new PlayerDefinition("Yellow Freak", Keys.NumPad2, Keys.NumPad3, Color.Yellow),
                new PlayerDefinition("Pink Coward", Keys.M, Keys.OemComma, Color.HotPink),
                new PlayerDefinition("Violent Violet", Keys.Add, Keys.Subtract, Color.DarkViolet),
                new PlayerDefinition("Mr. White", Keys.G, Keys.H, Color.White)
            };
        }
    }
}