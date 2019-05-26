using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameLoopSample
{
    public class GameText : IDisposable
    {
        private readonly IntPtr _rendererPtr;
        private readonly IntPtr _fontPtr;
        private SDL.SDL_Color _color;
        private IntPtr _surfacePtr;
        private IntPtr _texture;
        private string _text;


        public GameText(IntPtr renderer, string fontFilePath, string text, int fontSize, Color color)
        {
            _text = text;
            _rendererPtr = renderer;

            _fontPtr = SDL_ttf.TTF_OpenFont(fontFilePath, fontSize);

            _color = new SDL.SDL_Color
            {
                a = color.A,
                r = color.R,
                g = color.G,
                b = color.B
            };

            //Create a surface for which to render the text to
            _surfacePtr = SDL_ttf.TTF_RenderText_Solid(_fontPtr, text, _color);
            
            //Create a texture from the surface
            _texture = SDL.SDL_CreateTextureFromSurface(_rendererPtr, _surfacePtr);
        }


        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                if (_surfacePtr == IntPtr.Zero)
                    SDL.SDL_FreeSurface(_surfacePtr);

                //Create a surface for which to render the text to
                _surfacePtr = SDL_ttf.TTF_RenderText_Solid(_fontPtr, value, _color);

                //Remove the old texture pointer before creating a new one to prevent a memory leak
                if (_texture != IntPtr.Zero)
                    SDL.SDL_DestroyTexture(_texture);

                //Create a texture from the surface
                _texture = SDL.SDL_CreateTextureFromSurface(_rendererPtr, _surfacePtr);

                SDL.SDL_FreeSurface(_surfacePtr);
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public void Render()
        {
            SDL.SDL_QueryTexture(_texture, out var format, out var access, out var width, out var height);

            var srcRect = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = width,
                h = height
            };

            var destRect = new SDL.SDL_Rect()
            {
                x = X,
                y = Y,
                w = width,
                h = height
            };


            SDL.SDL_RenderCopy(_rendererPtr, _texture, ref srcRect, ref destRect);
        }


        public void Dispose()
        {
            SDL.SDL_FreeSurface(_surfacePtr);
            SDL.SDL_DestroyTexture(_texture);
            SDL_ttf.TTF_CloseFont(_fontPtr);
            SDL_ttf.TTF_Quit();
        }
    }
}
