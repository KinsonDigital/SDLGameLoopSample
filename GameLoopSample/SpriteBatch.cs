using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameLoopSample
{
    public class SpriteBatch
    {
        private IntPtr _rendererPtr;
        private bool _beginInvoked;


        public SpriteBatch(IntPtr rendererPtr) => _rendererPtr = rendererPtr;


        public void Begin() => _beginInvoked = true;

        public void End()
        {
            if (!_beginInvoked)
                throw new Exception($"The method {nameof(Begin)} must be invoked first before invoking method {nameof(End)}.");

            SDL.SDL_RenderPresent(_rendererPtr);
            _beginInvoked = false;
        }


        public void Clear(Color color)
        {
            if (!_beginInvoked)
                throw new Exception($"The method {nameof(Begin)} must be invoked first before invoking method {nameof(Clear)}.");

            SDL.SDL_SetRenderDrawColor(_rendererPtr, color.R, color.G, color.B, color.A);
            SDL.SDL_RenderClear(_rendererPtr);
        }


        public void Render(Texture texture, int x, int y)
        {
            SDL.SDL_SetTextureColorMod(texture.TexturePtr, 255, 255, 255);
            SDL.SDL_SetTextureAlphaMod(texture.TexturePtr, 255);
            SDL.SDL_SetTextureBlendMode(texture.TexturePtr, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

            var srcRect = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = texture.Width,
                h = texture.Height
            };

            var destRect = new SDL.SDL_Rect()
            {
                x = texture.X,
                y = texture.Y,
                w = texture.Width,
                h = texture.Height
            };

            //Render texture to screen
            SDL.SDL_RenderCopy(_rendererPtr, texture.TexturePtr, ref srcRect, ref destRect);
        }


        public void Render(GameText text, int x, int y)
        {
            var srcRect = new SDL.SDL_Rect()
            {
                x = 0,
                y = 0,
                w = text.Width,
                h = text.Height
            };

            var destRect = new SDL.SDL_Rect()
            {
                x = x,
                y = y,
                w = text.Width,
                h = text.Height
            };


            SDL.SDL_RenderCopy(_rendererPtr, text.TextPtr, ref srcRect, ref destRect);
        }
    }
}
