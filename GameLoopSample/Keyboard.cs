using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public static class Keyboard
    {
        private static List<SDL.SDL_Keycode> _downKeys = new List<SDL.SDL_Keycode>();


        public static bool IsKeyDown(SDL.SDL_Keycode key) => _downKeys.Contains(key);


        public static bool IsKeyUp(SDL.SDL_Keycode key) => !IsKeyDown(key);


        public static void AddKey(SDL.SDL_Keycode key)
        {
            if (!_downKeys.Contains(key))
                _downKeys.Add(key);
        }


        public static void RemoveKey(SDL.SDL_Keycode key) => _downKeys.Remove(key);
    }
}
