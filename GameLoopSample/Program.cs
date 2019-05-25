using SDL2;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace GameLoopSample
{
    public class Program
    {
        private static MyGame _game;


        static int Main(string[] args)
        {
            /*Basic info about minimum required to render text to the screen
             * http://gigi.nullneuron.net/gigilabs/displaying-text-in-sdl2-with-sdl_ttf/
             * 
             * Good place for free fonts: https://www.fontsquirrel.com/
             */

            _game = new MyGame();
            _game.Start();

            return 0;
        }
    }
}
