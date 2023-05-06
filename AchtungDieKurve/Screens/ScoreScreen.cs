using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using AchtungDieKurve.Game;
using AchtungDieKurve.Game.Drawable;
using AchtungDieKurve.Screens;

namespace AchtungDieKurve
{
    class ScoreScreen : GameScreen
    {

        SpriteBatch SB;
        Properties context;
        Texture2D pixel;



        private int frame_w;
    
        private int frame_x;

        private int block_w=170;
        private int max;
        private int min;
        private int avg;
        private int axisy;
        private int max_height;
        private const string gameover = "Game over!";
        private const string presscontinue = "(press SPACE to exit)";
        private Vector2 title_measure;
        private Vector2 continue_measure;
        private Vector2 boss_measure;
        private Vector2 looser_measure;
        private float spreadstep = 0.01f;
        private const int sq = 100;

        private int box_lpad=15, box_rpad=15, box_tpad=65, box_bpad=50;

        private ContentManager content;
        private SpriteFont smallfont;
        private SpriteFont bigfont;
        private PlayersManager players;
        private List<Kurve> ordered;// = new List<Kurve> { };

        public ScoreScreen(PlayersManager players)
        { 
            context=GameBase.Defaults;
            this.players = players;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            SB = new SpriteBatch(this.ScreenManager.GraphicsDevice);

            content = new ContentManager(ScreenManager.Game.Services, "Content");
            smallfont = content.Load<SpriteFont>("score_small_bold");
            bigfont= content.Load<SpriteFont>("score_big_bold");
    
            frame_w = players.Worms.Count * block_w;
            frame_x=((context.ScreenWidth-frame_w)/2)-sq/2;
            max_height = context.ScreenHeight - 300;
            pixel = new Texture2D(ScreenManager.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });


            ordered = (List<Kurve>)players.Worms.OrderByDescending(item => item.Score).ToList();
            min = ordered.Last().Score;
            max = ordered.First().Score;
            block_w = (frame_w / players.Worms.Count);// -sq / 4;
            avg = (min + max) / 2;

            axisy = (context.ScreenHeight / 2)-sq/2;

            continue_measure = smallfont.MeasureString(presscontinue);
            boss_measure = smallfont.MeasureString("BOSS");
            looser_measure = smallfont.MeasureString("LOSER");
            title_measure = bigfont.MeasureString(gameover);

        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            PlayerIndex px;
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Space,null,out px))
            {
            
            ScreenManager.RemoveScreen(this);
             LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
            }
        }


        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
            SB.Begin();

            int left = 0;//block_w;
            int sq_x;
            int sq_y;

            int ln_sq_x;
            int ln_sq_y;

            Vector2 fontmeasure;
            Rectangle r;
            Rectangle ln_r;
            int position=1;
            string position_str;
            int arr_pos = 0;
           
            //box
            SB.Draw(pixel,new Rectangle(box_lpad,box_tpad,context.ScreenWidth-box_rpad-box_lpad,context.ScreenHeight-box_bpad-box_tpad),Color.Gray);
            SB.Draw(pixel, new Rectangle(box_lpad+1, box_tpad+1, context.ScreenWidth - box_rpad - box_lpad-2, context.ScreenHeight - box_bpad - box_tpad-2), Color.FromNonPremultiplied(10,10,10,255));

            //avg line
            //DrawLine(new Vector2(box_lpad, (context.height - box_bpad - box_tpad) / 2 + box_tpad), new Vector2(context.width - box_rpad - box_lpad, (context.height - box_bpad - box_tpad) / 2 + box_tpad), SB, Color.Gray, 1);

            foreach (Kurve k in ordered)
            {
                

                sq_y = (int)(axisy + ((avg - k.Score) * (max_height / avg / players.Worms.Count) * spreadstep));
                sq_x = (int)(frame_x + (spreadstep * (left)));
                r = new Rectangle(sq_x, sq_y, sq, sq);
               
                left += block_w+sq/2;

                if (k != ordered.Last())
                {
                    
                    ln_sq_y = (int)(axisy + ((avg - ordered[arr_pos+1].Score) * (max_height / avg / players.Worms.Count) * spreadstep));
                    ln_sq_x = (int)(frame_x + (spreadstep * (left)));
                    ln_r = new Rectangle(ln_sq_x, ln_sq_y, sq, sq);
                    
                    DrawLine(new Vector2(r.Center.X,r.Center.Y),new Vector2(ln_r.Center.X,ln_r.Center.Y),SB,Color.DarkSlateGray,2);

                    arr_pos++;
                }
                SB.Draw(pixel, r, k.Color);
                fontmeasure=bigfont.MeasureString(k.Score.ToString());
                SB.DrawString(
                        bigfont,
                        k.Score.ToString(),
                        new Vector2((int)sq_x+(sq/2-fontmeasure.X/2),(int)sq_y+(sq/2-fontmeasure.Y/2)),
                        Color.Black
                        );
                position_str = "#" + position.ToString();
                SB.DrawString(
                    smallfont,
                    position_str,
                    new Vector2((int)sq_x+(int)(sq/2-smallfont.MeasureString(position_str).X/2),sq_y+10),
                    Color.Black
                    );

                if (k.Score == max)
                {
                    SB.DrawString(
                         smallfont,
                         "BOSS",
                         new Vector2((int)sq_x + (sq / 2 - boss_measure.X / 2), (int)sq_y + (sq / 2 - fontmeasure.Y / 2)+boss_measure.Y+10),
                         Color.Black
                         ); 
                }

                else
                if (k.Score == min)
                {
                    SB.DrawString(
                         smallfont,
                         "LOSER",
                         new Vector2((int)sq_x + (sq / 2 - looser_measure.X / 2), (int)sq_y + (sq / 2 - fontmeasure.Y / 2) + looser_measure.Y + 10),
                         Color.Black
                         );
                }

                position++;
            }

            

            SB.DrawString(ScreenManager.Font,"Aftermath!",
                new Vector2(((int)context.ScreenWidth/2)-(title_measure.X/2),20),
                Color.FromNonPremultiplied(255,160,0,255)
                );

            SB.DrawString(smallfont, presscontinue, new Vector2(15, context.ScreenHeight - 35), Color.FromNonPremultiplied(255, 160, 0, 255));

            SB.End();

            if (spreadstep<=1)
            spreadstep += 0.02f;
        }

        public void DrawLine(Vector2 start, Vector2 end, SpriteBatch SB, Color color, int thickness)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            SB.Draw(pixel,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    thickness), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}
