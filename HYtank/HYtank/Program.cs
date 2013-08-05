using System;
using System.Windows.Forms;

namespace HYtank
{
#if WINDOWS || XBOX
    static class Program
    {
        public static StartScreen startScreen = new StartScreen();

        static void Main(string[] args)
        {
            Application.Run(startScreen);
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}
        }
    }
#endif
}

