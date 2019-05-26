using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameLoopSample
{
    public class Game
    {
        private static IntPtr _windowPtr;
        private static IntPtr _rendererPtr;
        private int _windowWidth = 640;
        private int _windowHeight = 480;
        private Stopwatch _timer;
        private int _elapsedTime;
        private bool _isRunning;
        private int _timePerFrame = 1000 / 60;
        private Queue<double> _frameTimes = new Queue<double>();


        public Game(int windowWidth, int windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;
        }


        public IntPtr Renderer => _rendererPtr;

        public double FPS { get; set; }


        public void Start()
        {
            InitEngine();
            Initialize();
            Run();
        }


        public void Stop()
        {
            _timer.Stop();
            _isRunning = false;
        }


        private void Run()
        {
            _isRunning = true;
            _timer = new Stopwatch();
            _timer.Start();

            while(_isRunning)
            {
                //Check if the game has a signal to end
                while (SDL.SDL_PollEvent(out var e) != 0)
                {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                    {
                        _isRunning = false;
                        break;
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                    {
                        Keyboard.AddKey(e.key.keysym.sym);
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                    {
                        Keyboard.RemoveKey(e.key.keysym.sym);
                    }
                }

                _timer.Restart();

                var myTimer = new Stopwatch();
                myTimer.Start();
                Update();
                Render();
                myTimer.Stop();

                while(_timer.Elapsed.TotalMilliseconds <= _timePerFrame) { }

                var elapsedTime = _timer.Elapsed.TotalMilliseconds;

                //Add the frame time to the list of previous frame times
                _frameTimes.Enqueue(_timer.Elapsed.TotalMilliseconds);

                //If the list is full, dequeue the oldest item
                if (_frameTimes.Count >= 80)
                    _frameTimes.Dequeue();

                //Calculate the average frames per second
                FPS = Math.Round(1000 / _frameTimes.Average(), 2);
            }

            ShutDown();
        }


        public virtual void Initialize()
        {
        }


        public virtual void Update()
        {
        }


        public virtual void Render()
        {
        }


        #region Private Methods
        private void InitEngine()
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
                    _windowWidth, _windowHeight, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (_windowPtr == IntPtr.Zero)
                {
                    Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                    success = false;
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
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
        }


        private void ShutDown()
        {
            SDL.SDL_DestroyRenderer(_rendererPtr);
            SDL.SDL_DestroyWindow(_windowPtr);
            SDL.SDL_Quit();
        }
        #endregion
    }
}
