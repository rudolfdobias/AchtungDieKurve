using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AchtungDieKurve.Game;
using AchtungDieKurve.Game.Drawable;

namespace AchtungDieKurve.Sound
{
   public class SfxController
    {
        private List<SoundEffect> crashLight;
        private List<SoundEffect> crashMid;
        private List<SoundEffect> crashHeavy;

      
        private int lightCount = 8;
        private int midCount = 5;
        private int heavyCount = 3;

        private readonly Random _rnd;
      
        private bool sound_on = true;

        public void EnableSound()
        {
            sound_on = true;
        }

       public void DisableSound()
        {
            sound_on = false;
        }

        public bool SoundEnabled()
       {
           return sound_on;
       }
        

        public SfxController()
        {
            crashLight = new List<SoundEffect>() { };
            crashMid = new List<SoundEffect>() { };
            crashHeavy = new List<SoundEffect>() { };
            _rnd = new Random(DateTime.UtcNow.Millisecond);
        }

        public void Load()
        {
            for (int i = 0; i < lightCount; i++ )
                crashLight.Add(GameBase.GetInstance().Content.Load<SoundEffect>("Crashes/weak/"+i));
            for (int i = 0; i < midCount; i++)
                crashLight.Add(GameBase.GetInstance().Content.Load<SoundEffect>("Crashes/moderate/" + i));
            for (int i = 0; i < heavyCount; i++)
                crashLight.Add(GameBase.GetInstance().Content.Load<SoundEffect>("Crashes/heavy/" + i));
        }

        public void PlayRandomCrash(GameTime gt, Kurve player)
        {
            if (SoundEnabled() == false) return;

            PlayRandomWeak();
            return;
            

            /*var settings = GameBase.Settings;

            var delta=gt.TotalGameTime.TotalSeconds-GameBase.LastGameStarted.TotalSeconds;
            if (delta >= settings.MidCrashTimeDeltaSeconds)
            {
                this.PlayRandomHeavy();
            }
            else if (delta >= settings.WeakCrashTimeDeltaSeconds)
            {
                if (player.Diameter >= 18)
                    this.PlayRandomHeavy();
                else
                {
                    this.PlayRandomModerate();
                }
            }
            else
            {
                if (player.Diameter >= 18)
                    this.PlayRandomHeavy();
                else if (player.Diameter >= 13)
                    this.PlayRandomModerate();
                else
                    this.PlayRandomWeak();
            }*/
            
        }

       public void PlayRandomWeak()
        {

            int r = GameBase.Defaults.Rand.Next(crashLight.Count);

            crashLight[r].Play();
           
           
        }

       public void PlayRandomModerate()
       {
           int r = GameBase.Defaults.Rand.Next(crashMid.Count);

           crashMid[r].Play();
       }
       public void PlayRandomHeavy()
       {
           int r = GameBase.Defaults.Rand.Next(crashHeavy.Count);

           crashHeavy[r].Play();
       }
    }
}
