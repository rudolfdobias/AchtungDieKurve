using System;

namespace AchtungDieKurve
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameBase())
                game.Run();
        }
    }
}
