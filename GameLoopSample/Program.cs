using SDL2;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace GameLoopSample
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;

        private static IntPtr _windowPtr;
        private static IntPtr _rendererPtr;
        private static GameText _gameText;


        static int Main(string[] args)
        {
            /*Basic info about minimum required to render text to the screen
             * http://gigi.nullneuron.net/gigilabs/displaying-text-in-sdl2-with-sdl_ttf/
             * 
             * Good place for free fonts: https://www.fontsquirrel.com/
             */

            var quit = false;

            Init();

            _gameText = new GameText(_rendererPtr, "OpenSans-Regular.ttf", "Hello World", 25, Color.White);

            while (!quit)
            {
                while(SDL.SDL_PollEvent(out var e) != 0)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    {
                        quit = true;
                        break;
                    }
                }

                _gameText.Text = DateTime.Now.ToLongTimeString();

                SDL.SDL_RenderClear(_rendererPtr);

                _gameText.Render();

                SDL.SDL_RenderPresent(_rendererPtr);
            }

            SDL.SDL_DestroyRenderer(_rendererPtr);
            SDL.SDL_DestroyWindow(_windowPtr);

            _gameText.Dispose();

            SDL.SDL_Quit();

            return 0;
        }


        private static bool Init()
        {
            //Initialization flag
            var success = true;

            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                {
                    Console.WriteLine("Warning: Linear texture filtering not enabled!");
                }

                //Create window
                _windowPtr = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                    SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (_windowPtr== IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                    _rendererPtr = SDL.SDL_CreateRenderer(_windowPtr, -1, renderFlags);
                    if (_rendererPtr == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(_rendererPtr, 48, 48, 48, 255);

                        //Initialize PNG loading
                        var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG;
                        if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                        {
                            Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
                            success = false;
                        }

                        //Initialize SDL_ttf
                        if (SDL_ttf.TTF_Init() == -1)
                        {
                            Console.WriteLine("SDL_ttf could not initialize! SDL_ttf Error: {0}", SDL.SDL_GetError());
                            success = false;
                        }
                    }
                }
            }

            return success;
        }
    }
}
