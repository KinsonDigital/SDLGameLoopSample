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
        private Stopwatch _timer;
        private TimeSpan _lastFrameTime;
        private bool _isRunning;
        private float _targetFrameRate = 1000f / 60f;
        private Queue<float> _frameTimes = new Queue<float>();
        private TimeSpan _elapsedRenderTime;


        public Game()
        {
        }


        public IntPtr Renderer => _rendererPtr;

        public float CurrentFPS { get; private set; }

        public float DesiredFPS
        {
            get => 1000f / _targetFrameRate;
            set => _targetFrameRate = 1000f / value;
        }

        public TimeStepType TimeStep { get; set; } = TimeStepType.Fixed;


        public int WindowWidth
        {
            get
            {
                SDL.SDL_GetWindowSize(_windowPtr, out int w, out _);

                return w;
            }
            set
            {
                SDL.SDL_GetWindowSize(_windowPtr, out _, out int h);
                SDL.SDL_SetWindowSize(_windowPtr, value, h);
            }
        }


        public int WindowHeight
        {
            get
            {
                SDL.SDL_GetWindowSize(_windowPtr, out _, out int h);

                return h;
            }
            set
            {
                SDL.SDL_GetWindowSize(_windowPtr, out int w, out _);
                SDL.SDL_SetWindowSize(_windowPtr, w, value);
            }
        }


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
                if (TimeStep == TimeStepType.Fixed)
                {
                    if (_timer.Elapsed.TotalMilliseconds >= _targetFrameRate)
                    {
                        Update(_timer.Elapsed);
                        Render(_timer.Elapsed);

                        //Add the frame time to the list of previous frame times
                        _frameTimes.Enqueue((float)_timer.Elapsed.TotalMilliseconds);

                        //If the list is full, dequeue the oldest item
                        if (_frameTimes.Count >= 100)
                            _frameTimes.Dequeue();

                        //Calculate the average frames per second
                        CurrentFPS = (float)Math.Round(1000f / _frameTimes.Average(), 2);

                        _timer.Restart();
                    }
                }
                else if (TimeStep == TimeStepType.Variable)
                {
                    var currentFrameTime = _timer.Elapsed;
                    var elapsed = currentFrameTime - _lastFrameTime;

                    _lastFrameTime = currentFrameTime;

                    Update(elapsed);
                    Render(elapsed);

                    _timer.Stop();

                    //Add the frame time to the list of previous frame times
                    _frameTimes.Enqueue((float)elapsed.TotalMilliseconds);

                    //If the list is full, dequeue the oldest item
                    if (_frameTimes.Count >= 100)
                        _frameTimes.Dequeue();

                    //Calculate the average frames per second
                    CurrentFPS = (float)Math.Round(1000f / _frameTimes.Average(), 2);

                    _timer.Start();
                }
            }

            ShutDown();
        }


        public virtual void Initialize()
        {
        }


        public virtual void Update(TimeSpan elapsedTime)
        {

        }


        public virtual void Render(TimeSpan elapsedTime)
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
                    640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
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
