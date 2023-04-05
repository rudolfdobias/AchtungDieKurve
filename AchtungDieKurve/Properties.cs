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
        public const string RegistryPath = "AchtungDieKurve";
        public const string RegistrySettingsPath = "1.0";
        public bool DebugCoordinates = false;
        public bool DebugCollisions = false;
        public bool EnableLog = true;

        // Game configuration -----------------------------------------------     
        public int GoalPlusPerPlayer = 5;
        public int InitialProtectionTime = 2000;
        public bool PowerupsEnabled = true;

        // View -------------------------------------------------------------
        public int ScreenWidth = 1920;
        public int ScreenHeight = 1080;
        public float Scale = 1;

        // Player curve specs -----------------------------------------------
        public int MaxPlayerThickness = 100;
        public int MinPlayerThickness = 1;
        public bool WallBounceWhileProtection = true;
        public double HoleProbability = 0.009;
        public double HoleTerminationProbability = 0.1;
        public float DefaultSpeed = 2.2f;
        public float DefaultTurnStep = 0.05f;
        public int DefaultDiameter = 8;
        public float MinPlayerSpeed = 0.5f;
        public float MaxPlayerSpeed = 20f;

        // Powerups ---------------------------------------------------------
        public double PowerupProbability = 0.01;

        // Runtime vars -----------------------------------------------------
        public int GameGoal = 0;


        // OLD SHIT
        public Random Rand = new Random();
        public SfxController Sfx;

        // Sounds
        public int WeakCrashTimeDeltaSeconds = 15;
        public int MidCrashTimeDeltaSeconds = 30;
        public int HeavyCrashTimeDeltaSeconds = 60;


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