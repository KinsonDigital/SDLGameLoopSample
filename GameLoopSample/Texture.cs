using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public class Texture
    {
        private IntPtr _rendererPtr;
        private IntPtr _texturePtr;

        public Texture(IntPtr renderer, string textureName)
        {
            _rendererPtr = renderer;

            var texturePath = $@"Content\{textureName}.png";

            //Load image at specified path
            var loadedSurface = SDL_image.IMG_Load(texturePath);

            if (loadedSurface == IntPtr.Zero)
            {
                throw new Exception($"Unable to load image {texturePath}! SDL Error: {SDL.SDL_GetError()}");
            }
            else
            {
                //Create texture from surface pixels
                _texturePtr = SDL.SDL_CreateTextureFromSurface(_rendererPtr, loadedSurface);

                if (_texturePtr == IntPtr.Zero)
                    throw new Exception($"Unable to create texture from {texturePath}! SDL Error: {SDL.SDL_GetError()}");

                //Get rid of old loaded surface
                SDL.SDL_FreeSurface(loadedSurface);
            }
        }


        public int X { get; set; }

        public int Y { get; set; }


        public void Render()
        {
            SDL.SDL_QueryTexture(_texturePtr, out uint format, out int access, out int width, out int height);
            SDL.SDL_SetTextureColorMod(_texturePtr, 255, 255, 255);
            SDL.SDL_SetTextureAlphaMod(_texturePtr, 255);
            SDL.SDL_SetTextureBlendMode(_texturePtr, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

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

            //Render texture to screen
            SDL.SDL_RenderCopy(_rendererPtr, _texturePtr, ref srcRect, ref destRect);
        }
    }
}
