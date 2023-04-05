namespace AchtungDieKurve.Game.Core
{
    public static class ExtensionMethods
    {
        public static int WrapAngleDegrees(this int angle)
        {
            angle = Trigonometry.WrapAngleDegrees(angle);
            return angle;
        }
  
    }
}