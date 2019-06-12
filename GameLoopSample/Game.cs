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
        #region Fields
        private static IntPtr _windowPtr;
        private Stopwatch _timer;
        private TimeSpan _lastFrameTime;
        private bool _isRunning;
        private static float _targetFrameRate = 1000f / 60f;
        private Queue<float> _frameTimes = new Queue<float>();
        #endregion


        #region Constructors
        public Game()
        {
        }
        #endregion


        #region Props
        public static IntPtr Renderer { get; private set; }

        public static float CurrentFPS { get; private set; }

        public static float DesiredFPS
        {
            get => 1000f / _targetFrameRate;
            set => _targetFrameRate = 1000f / value;
        }

        public static TimeStepType TimeStep { get; set; } = TimeStepType.Fixed;

        public static int WindowWidth
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

        public static int WindowHeight
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

        internal static List<SDL.SDL_Keycode> CurrentStateKeys { get; set; } = new List<SDL.SDL_Keycode>();
        #endregion


        #region Public Methods
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


        public virtual void Initialize()
        {
        }


        public virtual void Update(TimeSpan elapsedTime)
        {

        }


        public virtual void Render(TimeSpan elapsedTime)
        {
        }
        #endregion


        #region Private Methods
        private void Run()
        {
            _isRunning = true;
            _timer = new Stopwatch();
            _timer.Start();

            while(_isRunning)
            {
                UpdateInputStates();

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


        private void UpdateInputStates()
        {
            //Check if the game has a signal to end
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    if (!CurrentStateKeys.Contains(e.key.keysym.sym))
                        CurrentStateKeys.Add(e.key.keysym.sym);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    CurrentStateKeys.Remove(e.key.keysym.sym);
                }
            }
        }


        private void InitEngine()
        {
            //Initialize SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                throw new Exception($"SDL could not initialize! SDL_Error: {SDL.SDL_GetError()}");
            }
            else
            {
                //Set texture filtering to linear
                if (SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "1") == SDL.SDL_bool.SDL_FALSE)
                    throw new Exception("Warning: Linear texture filtering not enabled!");

                //Create window
                _windowPtr = SDL.SDL_CreateWindow("SDL Tutorial", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                    640, 480, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
                if (_windowPtr == IntPtr.Zero)
                {
                    throw new Exception($"Window could not be created! SDL_Error: {SDL.SDL_GetError()}");
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED;
                    Renderer = SDL.SDL_CreateRenderer(_windowPtr, -1, renderFlags);

                    if (Renderer == IntPtr.Zero)
                    {
                        throw new Exception($"Renderer could not be created! SDL Error: {SDL.SDL_GetError()}");
                    }
                    else
                    {
                        //Initialize renderer color
                        SDL.SDL_SetRenderDrawColor(Renderer, 48, 48, 48, 255);

                        //Initialize PNG loading
                        var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG;

                        if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
                            throw new Exception($"SDL_image could not initialize! SDL_image Error: {SDL.SDL_GetError()}");

                        //Initialize SDL_ttf
                        if (SDL_ttf.TTF_Init() == -1)
                            throw new Exception($"SDL_ttf could not initialize! SDL_ttf Error: {SDL.SDL_GetError()}");
                    }
                }
            }
        }


        private void ShutDown()
        {
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(_windowPtr);
            SDL.SDL_Quit();
        }
        #endregion
    }
}
