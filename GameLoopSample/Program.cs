using SDL2;
using System;
using System.Globalization;
using System.Threading;

namespace GameLoopSample
{
    class Program
    {
        //Screen dimension constants
        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;

        //The window we'll be rendering to
        private static IntPtr _Window = IntPtr.Zero;

        //The surface contained by the window
        public static IntPtr Renderer = IntPtr.Zero;

        //Globally used font
        public static IntPtr Font = IntPtr.Zero;
        private static IntPtr _windowPtr;
        private static IntPtr _rendererPtr;


        //Rendered texture
        private static readonly LTexture _TextTexture = new LTexture();


        static int Main(string[] args)
        {
            /*Basic info about minimum required to render text to the screen
             * http://gigi.nullneuron.net/gigilabs/displaying-text-in-sdl2-with-sdl_ttf/
             * 
             * Good place for free fonts: https://www.fontsquirrel.com/
             */


            bool quit = false;

            SDL.SDL_Event e;

            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            SDL_ttf.TTF_Init();

            _windowPtr = SDL.SDL_CreateWindow("Game Loop", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            _rendererPtr = SDL.SDL_CreateRenderer(_windowPtr, -1, 0);
            var font = SDL_ttf.TTF_OpenFont("OpenSans-Regular.ttf", 25);
            IntPtr surface = IntPtr.Zero;
            IntPtr texture = IntPtr.Zero;

            while (!quit)
            {
                SDL.SDL_WaitEvent(out e);

                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        quit = true;
                        break;
                }

                var color = new SDL.SDL_Color();
                color.a = 255;
                color.r = 255;
                color.g = 255;
                color.b = 255;

                //Create a surface for which to render the text to
                surface = SDL_ttf.TTF_RenderText_Solid(font, "Hello World", color);

                //Create a texture from the surface
                texture = SDL.SDL_CreateTextureFromSurface(_rendererPtr, surface);

                SDL.SDL_QueryTexture(texture, out uint format, out int access, out int width, out int height);

                var srcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = width,
                    h = height
                };

                var destRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = width,
                    h = height
                };

                SDL.SDL_RenderCopy(_rendererPtr, texture, ref srcRect, ref destRect);
                SDL.SDL_RenderPresent(_rendererPtr);
            }

            SDL.SDL_DestroyTexture(texture);
            SDL.SDL_FreeSurface(surface);
            SDL.SDL_DestroyRenderer(_rendererPtr);
            SDL.SDL_DestroyWindow(_windowPtr);
            SDL_ttf.TTF_CloseFont(font);
            SDL_ttf.TTF_Quit();
            SDL.SDL_Quit();

            return 0;
        }


        private static bool Init()
        {
            //Initialization flag
            bool success = true;

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
                _Window = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
                    SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (_Window == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
                    Renderer = SDL.SDL_CreateRenderer(_Window, -1, renderFlags);
                    if (Renderer == IntPtr.Zero)
                    {
                        Console.WriteLine("Renderer could not be created! SDL Error: {0}", SDL.SDL_GetError());
                        success = false;
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);

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


        static bool LoadMedia()
        {
            //Loading success flag
            bool success = true;

            //Open the font
            Font = SDL_ttf.TTF_OpenFont("lazy.ttf", 28);
            if (Font == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load lazy font! SDL_ttf Error: {0}", SDL.SDL_GetError());
                success = false;
            }
            else
            {
                //Render text
                var textColor = new SDL.SDL_Color();
                if (!_TextTexture.LoadFromRenderedText("The quick brown fox jumps over the lazy dog", textColor))
                {
                    Console.WriteLine("Failed to render text texture!");
                    success = false;
                }
            }

            return success;
        }


        private static void Close()
        {
            //Free loaded images
            _TextTexture.Free();

            //Free global font
            SDL_ttf.TTF_CloseFont(Font);
            Font = IntPtr.Zero;

            //Destroy window
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_Window);
            _Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;

            //Quit SDL subsystems
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            SDL.SDL_Quit();
        }

    }
}
