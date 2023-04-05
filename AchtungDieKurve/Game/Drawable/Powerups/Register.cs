using System.Collections.Generic;

namespace AchtungDieKurve.Game.Drawable.Powerups
{
    public struct PowerupInfo
    {
        public PowerupInfo(double probability, string name) : this()
        {
            Probability = probability;
            Name = name;
        }

        public string Name { get; set; }

        public double Probability { get; set; }
    }

    public class Register
    {
        public static List<PowerupInfo> Powerups;
        

        public static void Load()
        {
            Powerups = new List<PowerupInfo>
            {
               
              // new PowerupInfo(0.2f, "Clear"),
                new PowerupInfo(0.4f, "Fast"),
                new PowerupInfo(0.5f, "FastEnemy"),
                new PowerupInfo(0.5f, "SlowEnemy"),
                new PowerupInfo(0.5f, "SlimEnemy"),
                new PowerupInfo(0.5f, "FatEnemy"),
                new PowerupInfo(0.5f, "Fat"),
                new PowerupInfo(0.3f, "Transcend"),
                new PowerupInfo(0.5f, "Slim"),
                new PowerupInfo(0.4f, "Slow"),
                new PowerupInfo(0.1f, "Death"),
                new PowerupInfo(12f, "NoWalls"),
                //new PowerupInfo(1f, "Twister"),
                new PowerupInfo(0.3f, "Switch")
            };
        }
    }
}

