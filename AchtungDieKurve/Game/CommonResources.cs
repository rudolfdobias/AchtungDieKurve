
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AchtungDieKurve.Game
{
    public class CommonResources
    {
        private static ContentManager content;
        public static Texture2D whitepixel;
        public static Texture2D blackpixel;
        public static Texture2D blankTexture;
        public static Texture2D kurveBody;
        public static Texture2D starEffect;
        public static SpriteFont fontMedium;
        public static SpriteFont fontSmall;
        public static SpriteFont fontSmallBold;
        public static SpriteFont fontSmaller;
        public static SpriteFont fontBig;
        public static Color TransparentOverlayColor;
        public static Color Background;
        public static Color Borders;
        public static Color TableBodyColor;
        public static Color Emhasis;
        public static Color MainColor;
        public static Color TextColor;

        public static void Load(GameBase game)
        {
            if (content == null)
                content = new ContentManager(game.Services, "Content");

            // textures
            whitepixel = (new Texture2D(GameBase.Graphics.GraphicsDevice, 1, 1));
            whitepixel.SetData(new[] { Color.White });
            blackpixel = new Texture2D(GameBase.Graphics.GraphicsDevice, 1, 1);
            blackpixel.SetData(new[] { Color.Black });
            kurveBody = content.Load<Texture2D>("body");


            // fonts
            fontSmall = content.Load<SpriteFont>("fontSmall");
            fontSmallBold = content.Load<SpriteFont>("fontSmallBold");
            fontMedium = content.Load<SpriteFont>("fontMedium");
            fontBig = content.Load<SpriteFont>("menufont");
            fontSmaller = content.Load<SpriteFont>("copyfont");
            blankTexture = content.Load<Texture2D>("blank");
            starEffect = content.Load<Texture2D>("star");

            // common colors
            Background = Color.FromNonPremultiplied(20, 20, 20, 255);
            TransparentOverlayColor = Color.FromNonPremultiplied(0, 0, 10, 200);
            Borders = Color.FromNonPremultiplied(130, 130, 130, 255);
            TableBodyColor = Color.FromNonPremultiplied(40, 40, 40, 255);
            Emhasis = Color.White; // Color.FromNonPremultiplied(200, 140, 0, 255);
            MainColor = Color.FromNonPremultiplied(241, 47, 0, 240);
            TextColor = Color.FromNonPremultiplied(240, 240, 240, 255);
        }
    }
}
