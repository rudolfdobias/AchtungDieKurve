using System.Collections.Generic;
using AchtungDieKurve.Graphics;
using Microsoft.Xna.Framework;

namespace AchtungDieKurve
{
    /// <summary>
    /// Video settings menu: fullscreen toggle and window size selection.
    /// </summary>
    class VideoScreen : MenuScreen
    {
        MenuEntry adapterEntry;
        MenuEntry fullscreenEntry;
        MenuEntry resolutionEntry;

        List<Point> windowSizes;
        int windowSizeIndex;

        public VideoScreen(ScreenManager screenManager)
            : base("Video")
        {
            ScreenManager = screenManager;

            adapterEntry = new MenuEntry(string.Empty);
            fullscreenEntry = new MenuEntry(string.Empty);
            resolutionEntry = new MenuEntry(string.Empty);
            var back = new MenuEntry("Back");

            windowSizes = GraphicsManager.AvailableWindowSizes();
            windowSizeIndex = windowSizes.IndexOf(GameBase.GraphicsManager.WindowedSize);
            if (windowSizeIndex < 0)
                windowSizeIndex = windowSizes.Count - 1;

            fullscreenEntry.Selected += FullscreenSelected;
            resolutionEntry.Selected += ResolutionSelected;
            back.Selected += OnCancel;

            MenuEntries.Add(adapterEntry);
            MenuEntries.Add(fullscreenEntry);
            MenuEntries.Add(resolutionEntry);
            MenuEntries.Add(back);

            SetMenuEntryText();
        }

        void SetMenuEntryText()
        {
            var manager = GameBase.GraphicsManager;

            adapterEntry.Text = ScreenManager.GraphicsDevice.Adapter.Description;
            fullscreenEntry.Text = "Fullscreen: " + (manager.IsFullScreen ? "ON" : "OFF");

            if (manager.IsFullScreen)
            {
                var desktop = GraphicsManager.DesktopResolution;
                resolutionEntry.Text = "Resolution: " + desktop.X + " x " + desktop.Y + " (desktop)";
            }
            else
            {
                var size = windowSizes[windowSizeIndex];
                resolutionEntry.Text = "Resolution: " + size.X + " x " + size.Y;
            }
        }

        void FullscreenSelected(object sender, PlayerIndexEventArgs e)
        {
            GameBase.GraphicsManager.SetFullScreen(!GameBase.GraphicsManager.IsFullScreen);
            SetMenuEntryText();
        }

        /// <summary>
        /// Cycles through the available window sizes; fullscreen always uses
        /// the desktop resolution, so the entry is informational there.
        /// </summary>
        void ResolutionSelected(object sender, PlayerIndexEventArgs e)
        {
            if (GameBase.GraphicsManager.IsFullScreen)
                return;

            windowSizeIndex = (windowSizeIndex + 1) % windowSizes.Count;
            GameBase.GraphicsManager.SetWindowedSize(windowSizes[windowSizeIndex]);
            SetMenuEntryText();
        }
    }
}
