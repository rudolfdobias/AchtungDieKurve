using System;
using Microsoft.Xna.Framework;

namespace AchtungDieKurve.Game.Core
{
    public static class Trigonometry
    {
        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            return Math.Atan2(
                a.Y - b.Y,
                a.X - b.X
                );
        }

        public static double AngleBetween(Point a, Point b)
        {
            return Math.Atan2(
                a.Y - b.Y,
                a.X - b.X
                );
        }

        public static double AnglesCenter(double alpha, double beta)
        {
            var difference = beta - alpha;
            while (difference < -180) difference += 360;
            while (difference > 180) difference -= 360;
            return difference;
        }

        public static int WrapAngleDegrees(int angle)
        {
            var num = angle;
            if (num > 180)
            {
                num = -1 * (num - 180);
            }
            return num;
        }

        public static bool AngleInRange(double angle, double from, double to)
        {
            return Math.Cos(from)*Math.Cos(to) + Math.Sin(from)*Math.Sin(to) >= Math.Cos(angle);
        }

        public static bool AngleInRange(int angle, int from, int to)
        {
            return Math.Cos(MathHelper.ToRadians(from))*Math.Cos(MathHelper.ToRadians(to)) +
                   Math.Sin(MathHelper.ToRadians(from))*Math.Sin(MathHelper.ToRadians(to)) >=
                   Math.Cos(MathHelper.ToRadians(angle));
        }
    }
}