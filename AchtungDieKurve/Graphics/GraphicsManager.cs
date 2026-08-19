using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AchtungDieKurve.Graphics
{
    /// <summary>
    /// Owns display mode state. Fullscreen is always borderless at the desktop
    /// resolution (exclusive mode switches are unreliable on macOS); windowed
    /// mode uses a selectable window size.
    /// </summary>
    public class GraphicsManager
    {
        // Gameplay tuning baseline: values in Properties are calibrated for 1080p.
        private const int ReferenceHeight = 1080;
        private const int ReferenceDiameter = 8;
        private const float ReferenceSpeed = 2.2f;

        private static readonly Point[] CommonWindowSizes =
        {
            new Point(1024, 576),
            new Point(1280, 720),
            new Point(1366, 768),
            new Point(1600, 900),
            new Point(1920, 1080),
            new Point(2560, 1440),
            new Point(3440, 1440),
            new Point(3840, 2160),
        };

        private readonly Properties context;
        private Point windowedSize;

        public GraphicsManager(GameBase game, Properties context)
        {
            this.context = context;
        }

        public bool IsFullScreen => GameBase.Graphics.IsFullScreen;

        public Point WindowedSize => windowedSize;

        public static Point DesktopResolution
        {
            get
            {
                var mode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                return new Point(mode.Width, mode.Height);
            }
        }

        /// <summary>
        /// Window sizes selectable in windowed mode: common 16:9 sizes plus the
        /// adapter's modes, all fitting the desktop, smallest first.
        /// </summary>
        public static List<Point> AvailableWindowSizes()
        {
            var desktop = DesktopResolution;
            return CommonWindowSizes
                .Concat(GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                    .Select(m => new Point(m.Width, m.Height)))
                .Where(p => p.X <= desktop.X && p.Y <= desktop.Y)
                .Distinct()
                .OrderBy(p => (long)p.X * p.Y)
                .ToList();
        }

        public void Reset()
        {
            windowedSize = DefaultWindowedSize();
            SetFullScreen(true);
        }

        public void SetFullScreen(bool on)
        {
            ApplyMode(on, on ? DesktopResolution : windowedSize);
        }

        /// <summary>
        /// Remembers the window size and applies it immediately when windowed.
        /// </summary>
        public void SetWindowedSize(Point size)
        {
            windowedSize = size;
            if (!IsFullScreen)
                ApplyMode(false, size);
        }

        private void ApplyMode(bool fullScreen, Point size)
        {
            GameBase.Graphics.HardwareModeSwitch = false;
            GameBase.Graphics.IsFullScreen = fullScreen;
            GameBase.Graphics.PreferredBackBufferWidth = size.X;
            GameBase.Graphics.PreferredBackBufferHeight = size.Y;
            GameBase.Graphics.ApplyChanges();

            context.ScreenWidth = size.X;
            context.ScreenHeight = size.Y;
            ScaleGameplay(size.Y);
        }

        private void ScaleGameplay(int height)
        {
            var ratio = (float)height / ReferenceHeight;
            context.DefaultDiameter = Math.Max(2, (int)Math.Round(ReferenceDiameter * ratio));
            context.DefaultSpeed = ReferenceSpeed * ratio;
        }

        // Largest available size that still leaves room around the window.
        private static Point DefaultWindowedSize()
        {
            var desktop = DesktopResolution;
            var fitting = AvailableWindowSizes()
                .Where(p => p.X < desktop.X && p.Y < desktop.Y)
                .ToList();
            return fitting.Count > 0 ? fitting[fitting.Count - 1] : desktop;
        }
    }
}
