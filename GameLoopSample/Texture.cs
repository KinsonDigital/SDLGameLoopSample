using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public class Texture
    {
        private IntPtr _rendererPtr;

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
                TexturePtr = SDL.SDL_CreateTextureFromSurface(_rendererPtr, loadedSurface);

                SDL.SDL_QueryTexture(TexturePtr, out uint format, out int access, out int width, out int height);

                Width = width;
                Height = height;

                if (TexturePtr == IntPtr.Zero)
                    throw new Exception($"Unable to create texture from {texturePath}! SDL Error: {SDL.SDL_GetError()}");

                //Get rid of old loaded surface
                SDL.SDL_FreeSurface(loadedSurface);
            }
        }

        public IntPtr TexturePtr { get; set; }

        public int Width { get; private set; }

        public int Height { get; private set; }
    }
}
