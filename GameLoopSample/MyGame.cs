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
        private GameText _currentFPS;
        private GameText _desiredFPS;
        private GameText _gameLoopType;
        private GameText _startText;
        private GameText _finishText;
        private GameText _distanceText;
        private Texture _boxFace;
        private SpriteBatch _spriteBatch;
        private Stopwatch _timer = new Stopwatch();
        private int _finishLineX = 350;
        private double _timeElapsed;
        private Vector _boxFaceVel = new Vector(100, 200);


        public MyGame()
        {
            TimeStep = TimeStepType.Fixed;
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Renderer);

            _startText = new GameText(Renderer, "OpenSans-Regular.ttf", "Start", 14, Color.White);
            _finishText = new GameText(Renderer, "OpenSans-Regular.ttf", "Finish", 14, Color.White);
            _distanceText = new GameText(Renderer, "OpenSans-Regular.ttf", "Distance: 200 px", 14, Color.White);
            _currentFPS = new GameText(Renderer, "OpenSans-Regular.ttf", string.Empty, 14, Color.White);
            _desiredFPS = new GameText(Renderer, "OpenSans-Regular.ttf", $"Desired FPS: {DesiredFPS}", 14, Color.White);
            _gameLoopType = new GameText(Renderer, "OpenSans-Regular.ttf", $"Game Loop Type: {TimeStep.ToString()}", 14, Color.White);

            _boxFace = new Texture(Renderer, "OrangeBox");

            base.Initialize();
        }


        public override void Update(TimeSpan elapsedTime)
        {
            Keyboard.UpdateCurrentState();

            _currentFPS.Text = $"Current FPS: {CurrentFPS}";

            if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                if (!_timer.IsRunning && _boxFaceVel.X + _boxFace.Width < _finishLineX)
                    _timer.Start();

                var delta = (float)elapsedTime.TotalMilliseconds / 1000f;

                _boxFaceVel.X += 30.3f * delta;
            }

            if (Keyboard.WasKeyPressed(SDL.SDL_Keycode.SDLK_UP))
            {
                DesiredFPS += 0.5f;
                _desiredFPS.Text = $"Desired FPS: {Math.Round(DesiredFPS, 2)}";

                WindowHeight += 20;
            }


            if (Keyboard.WasKeyPressed(SDL.SDL_Keycode.SDLK_DOWN))
            {
                DesiredFPS -= 0.5f;
                _desiredFPS.Text = $"Desired FPS: {Math.Round(DesiredFPS, 2)}";

                WindowHeight -= 20;
            }


            if (_timer.IsRunning && _boxFaceVel.X + _boxFace.Width >= _finishLineX)
            {
                _timer.Stop();
                _timeElapsed = Math.Round(_timer.Elapsed.TotalMilliseconds, 2);
                _finishText.Text = $"Finish(ms): {_timeElapsed}";
            }


            Keyboard.UpdatePreviousState();

            base.Update(elapsedTime);
        }


        public override void Render(TimeSpan elapsedTime)
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

            //Render the desired FPS text
            _spriteBatch.Render(_desiredFPS, 5, 5);

            //Render the current FPS
            _spriteBatch.Render(_currentFPS, 5, 20);

            //Render the type of game loop
            _spriteBatch.Render(_gameLoopType, 5, 35);

            //Render box face!!
            _spriteBatch.Render(_boxFace, (int)_boxFaceVel.X, (int)_boxFaceVel.Y);

            _spriteBatch.End();

            base.Render(elapsedTime);
        }
    }
}
