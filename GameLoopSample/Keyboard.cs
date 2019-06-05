using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public static class Keyboard
    {
        #region Private Fields
        private static List<SDL.SDL_Keycode> _currentStateKeys = new List<SDL.SDL_Keycode>();
        private static List<SDL.SDL_Keycode> _previousStateKeys = new List<SDL.SDL_Keycode>();
        #endregion


        #region Public Methods
        public static bool IsKeyDown(SDL.SDL_Keycode key) => _currentStateKeys.Contains(key);


        public static bool IsKeyUp(SDL.SDL_Keycode key) => !IsKeyDown(key);


        public static bool WasKeyPressed(SDL.SDL_Keycode key)
        {
            return !_currentStateKeys.Contains(key) && _previousStateKeys.Contains(key);
        }


        public static void UpdateCurrentState()
        {
            _currentStateKeys.Clear();
            _currentStateKeys.AddRange(Game.CurrentStateKeys);
        }


        public static void UpdatePreviousState()
        {
            _previousStateKeys.Clear();
            _previousStateKeys.AddRange(_currentStateKeys);
        }
        #endregion
    }
}
