using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace GameLoopSample.Scenes
{
    public class RenderPerfScene : GameScene
    {
        private GameText _currentFPS;
        private GameText _averageRenderTime;
        private List<Texture> _textures = new List<Texture>();
        private Dictionary<int, bool> _textureTravelDirections = new Dictionary<int, bool>();
        private int _totalTextures;
        private float _averageRenderTimeValue;
        private readonly Queue<float> _renderTimes = new Queue<float>();


        public RenderPerfScene(int totalTextures) => _totalTextures = totalTextures;


        public override void Initialize()
        {
            Game.TimeStep = TimeStepType.Variable;

            _currentFPS = new GameText(Game.Renderer, "OpenSans-Regular.ttf", string.Empty, 14, Color.White);
            _averageRenderTime = new GameText(Game.Renderer, "OpenSans-Regular.ttf", string.Empty, 14, Color.White);

            var random = new Random();

            for (int i = 0; i < _totalTextures; i++)
            {
                var newTexture = new Texture(Game.Renderer, "BoxFace")
                {
                    Name = $"Texture-{i}",
                    X = random.Next(0, Game.WindowWidth - 50),
                    Y = 100,
                    Color = Color.FromArgb(
                        255,
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256),
                        (byte)random.Next(0, 256)
                    )
                };

                _textures.Add(newTexture);

                _textureTravelDirections.Add(newTexture.GetHashCode(), random.Next(1, 3) == 1);
            }

            base.Initialize();
        }


        public override void Update(TimeSpan elapsedTime)
        {
            _currentFPS.Text = $"Current FPS: {Game.CurrentFPS}";
            _averageRenderTime.Text = $"Ave. Render Time: {_averageRenderTimeValue}";

            for (int i = 0; i < _textures.Count; i++)
            {
                _textures[i].X = _textureTravelDirections[_textures[i].GetHashCode()] ? _textures[i].X + 1 : _textures[i].X - 1;

                if (_textures[i].X + _textures[i].Width > Game.WindowWidth)
                {
                    _textureTravelDirections[_textures[i].GetHashCode()] = false;
                }
                else if (_textures[i].X < 0)
                {
                    _textureTravelDirections[_textures[i].GetHashCode()] = true;
                }
            }

            base.Update(elapsedTime);
        }


        public override void Render(TimeSpan elapsedTime)
        {
            //Render the current frames per second text
            SceneManager.SpriteBatch.Render(_currentFPS, 5, 5);
            SceneManager.SpriteBatch.Render(_averageRenderTime, 5, 20);

            var timer = new Stopwatch();

            timer.Start();

            for (int i = 0; i < _textures.Count; i++)
            {
                SDL.SDL_SetTextureColorMod(_textures[i].TexturePtr, _textures[i].Color.R, _textures[i].Color.G, _textures[i].Color.B);
                SDL.SDL_SetTextureAlphaMod(_textures[i].TexturePtr, _textures[i].Color.A);
                SDL.SDL_SetTextureBlendMode(_textures[i].TexturePtr, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);

                timer.Stop();

                var srcRect = new SDL.SDL_Rect()
                {
                    x = 0,
                    y = 0,
                    w = _textures[i].Width,
                    h = _textures[i].Height
                };

                var destRect = new SDL.SDL_Rect()
                {
                    x = _textures[i].X,
                    y = _textures[i].Y,
                    w = _textures[i].Width,
                    h = _textures[i].Height
                };

                timer.Start();

                //Render texture to screen
                SDL.SDL_RenderCopy(Game.Renderer, _textures[i].TexturePtr, ref srcRect, ref destRect);
            }

            SDL.SDL_RenderPresent(Game.Renderer);

            timer.Stop();

            if (_renderTimes.Count >= 100)
                _renderTimes.Dequeue();

            _renderTimes.Enqueue((float)timer.Elapsed.TotalMilliseconds);

            _averageRenderTimeValue = (float)Math.Round(_renderTimes.Average(), 2);

            base.Render(elapsedTime);
        }
    }
}
