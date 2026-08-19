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
                new(0.4f, "Fast"),
                new(0.5f, "FastEnemy"),
                new(0.5f, "SlowEnemy"),
                new(0.5f, "SlimEnemy"),
                new(0.5f, "FatEnemy"),
                new(0.5f, "Fat"),
                new(0.3f, "Transcend"),
                new(0.5f, "Slim"),
                new(0.4f, "Slow"),
                new(0.1f, "Death"),
                new(0.3f, "NoWalls"),
                //new PowerupInfo(1f, "Twister"),
                new(0.3f, "Switch")
            };
        }
    }
}

