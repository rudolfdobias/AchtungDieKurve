namespace AchtungDieKurve.Graphics
{
    public class GraphicsManager
    {
        private Properties context;

        public GraphicsManager(GameBase game, Properties context)
        {
            this.context = context;
        }

        public void Reset()
        {
            SetDisplayMode(1920, 1080);
            SetFullScreen(true);
            SetScale();
            Apply();
        }

        public void SetScale(float scale = 1)
        {
            context.Scale = scale;
        }

        public void SetDisplayMode(int w, int h)
        {
            GameBase.Graphics.PreferredBackBufferWidth = w;
            GameBase.Graphics.PreferredBackBufferHeight = h;
            
            
            Apply();
        }

        public void Apply()
        {
            GameBase.Graphics.ApplyChanges();

            if (GameBase.Graphics.IsFullScreen)
            {
                context.ScreenWidth = GameBase.Graphics.GraphicsDevice.DisplayMode.Width;
                context.ScreenHeight = GameBase.Graphics.GraphicsDevice.DisplayMode.Height;
            }
            else
            {
                context.ScreenWidth = GameBase.Graphics.GraphicsDevice.Viewport.Width;
                context.ScreenHeight = GameBase.Graphics.GraphicsDevice.Viewport.Height;
            }
        }

        public void SetFullScreen(bool on = true)
        {
            if (GameBase.Graphics.IsFullScreen != on)
            {
                GameBase.Graphics.ToggleFullScreen();
                Apply();
            }
        }
    }
}
