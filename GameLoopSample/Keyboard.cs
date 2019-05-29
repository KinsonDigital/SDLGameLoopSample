using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public static class Keyboard
    {
        private static List<SDL.SDL_Keycode> _currentStateKeys = new List<SDL.SDL_Keycode>();
        private static List<SDL.SDL_Keycode> _previousStateKeys = new List<SDL.SDL_Keycode>();


        public static bool IsKeyDown(SDL.SDL_Keycode key) => _currentStateKeys.Contains(key);


        public static bool IsKeyUp(SDL.SDL_Keycode key) => !IsKeyDown(key);


        public static bool WasKeyPressed(SDL.SDL_Keycode key)
        {
            return !_currentStateKeys.Contains(key) && _previousStateKeys.Contains(key);
        }

        public static void UpdateCurrentState()
        {
            //Check if the game has a signal to end
            while (SDL.SDL_PollEvent(out var e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    if (!_currentStateKeys.Contains(e.key.keysym.sym))
                        _currentStateKeys.Add(e.key.keysym.sym);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    _currentStateKeys.Remove(e.key.keysym.sym);
                }
            }
        }


        public static void UpdatePreviousState()
        {
            _previousStateKeys.Clear();
            _previousStateKeys.AddRange(_currentStateKeys);
        }
    }
}
