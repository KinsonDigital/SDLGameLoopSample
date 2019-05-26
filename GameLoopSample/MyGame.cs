using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace GameLoopSample
{
    public class MyGame : Game
    {
        private GameText _gameText;
        private GameText _startText;
        private GameText _finishText;
        private GameText _distanceText;
        private Texture _boxFace;
        private SpriteBatch _spriteBatch;
        private Stopwatch _timer = new Stopwatch();
        private int _startTime;
        private int _finishLineX = 350;
        private double _timeElapsed;


        public MyGame() : base(640, 480)
        {
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Renderer);

            _startText = new GameText(Renderer, "OpenSans-Regular.ttf", "Start", 14, Color.White);
            _finishText = new GameText(Renderer, "OpenSans-Regular.ttf", "Finish", 14, Color.White);
            _distanceText = new GameText(Renderer, "OpenSans-Regular.ttf", "Distance: 200 px", 14, Color.White);
            _gameText = new GameText(Renderer, "OpenSans-Regular.ttf", string.Empty, 12, Color.White);

            _boxFace = new Texture(Renderer, "OrangeBox")
            {
                X = 100,
                Y = 200
            };

            base.Initialize();
        }


        public override void Update()
        {
            _gameText.Text = $"FPS: {FPS}";

            if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                if (!_timer.IsRunning && _boxFace.X + _boxFace.Width < _finishLineX)
                    _timer.Start();

                _boxFace.X += 1;
            }

            if (_timer.IsRunning && _boxFace.X + _boxFace.Width >= _finishLineX)
            {
                _timer.Stop();
                _timeElapsed = Math.Round(_timer.Elapsed.TotalMilliseconds, 2);
                _finishText.Text = $"Finish(ms): {_timeElapsed}";
            }


            base.Update();
        }


        public override void Render()
        {
            _spriteBatch.Begin();

            _spriteBatch.Clear(Color.FromArgb(255, 48, 48, 48));

            //Render the start line
            _spriteBatch.Render(_startText, 135, 150);
            _spriteBatch.RenderLine(150, 175, 150, 275, Color.Green);

            //Render the finish line
            _spriteBatch.Render(_finishText, 335, 150);
            _spriteBatch.RenderLine(_finishLineX, 175, _finishLineX, 275, Color.Yellow);

            //Render the distance text
            _spriteBatch.Render(_distanceText, 250 - (_distanceText.Width / 2), 300);

            _spriteBatch.Render(_boxFace, _boxFace.X, _boxFace.Y);
            _spriteBatch.Render(_gameText, 5, 5);

            _spriteBatch.End();

            base.Render();
        }
    }
}
