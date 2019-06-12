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
        private int _totalSamples = 2000;
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

            _averageRenderTime.Color = _renderTimes.Count >= _totalSamples ?
                Color.Green :
                Color.White;

            SceneManager.SpriteBatch.Render(_averageRenderTime, 5, 20);

            var timer = new Stopwatch();
            timer.Start();

            for (int i = 0; i < _textures.Count; i++)
            {
                SceneManager.SpriteBatch.Render(_textures[i]);
            }

            SDL.SDL_RenderPresent(Game.Renderer);

            timer.Stop();
            RecordTime(timer.Elapsed.TotalMilliseconds);

            base.Render(elapsedTime);
        }


        private void RecordTime(double time)
        {
            if (_renderTimes.Count >= _totalSamples)
                _renderTimes.Dequeue();

            _renderTimes.Enqueue((float)time);

            _averageRenderTimeValue = (float)Math.Round(_renderTimes.Average(), 2);
        }
    }
}
