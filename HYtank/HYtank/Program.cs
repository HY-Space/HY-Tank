using System;
using System.Windows.Forms;

namespace HYtank
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Application.Run(new StartScreen());
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}
        }
    }
#endif
}

