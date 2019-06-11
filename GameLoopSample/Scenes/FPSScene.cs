using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace GameLoopSample.Scenes
{
    public class FPSScene : GameScene
    {
        private GameText _currentFPS;
        private GameText _desiredFPS;
        private GameText _gameLoopType;
        private GameText _startText;
        private GameText _finishText;
        private GameText _distanceText;
        private Texture _boxFace;
        private Stopwatch _timer = new Stopwatch();
        private int _finishLineX = 350;
        private double _timeElapsed;
        private Vector _boxFaceVel = new Vector(100, 200);


        public override void Initialize()
        {
            _startText = new GameText(Game.Renderer, "OpenSans-Regular.ttf", "Start", 14, Color.White);
            _finishText = new GameText(Game.Renderer, "OpenSans-Regular.ttf", "Finish", 14, Color.White);
            _distanceText = new GameText(Game.Renderer, "OpenSans-Regular.ttf", "Distance: 200 px", 14, Color.White);
            _currentFPS = new GameText(Game.Renderer, "OpenSans-Regular.ttf", string.Empty, 14, Color.White);
            _desiredFPS = new GameText(Game.Renderer, "OpenSans-Regular.ttf", $"Desired FPS: {Game.DesiredFPS}", 14, Color.White);
            _gameLoopType = new GameText(Game.Renderer, "OpenSans-Regular.ttf", $"Game Loop Type: {Game.TimeStep.ToString()}", 14, Color.White);

            _boxFace = new Texture(Game.Renderer, "BoxFace");

            base.Initialize();
        }


        public override void Update(TimeSpan elapsedTime)
        {
            Keyboard.UpdateCurrentState();

            _currentFPS.Text = $"Current FPS: {Game.CurrentFPS}";

            if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                if (!_timer.IsRunning && _boxFaceVel.X + _boxFace.Width < _finishLineX)
                    _timer.Start();

                var delta = (float)elapsedTime.TotalMilliseconds / 1000f;

                _boxFaceVel.X += 30.3f * delta;
            }

            if (Keyboard.WasKeyPressed(SDL.SDL_Keycode.SDLK_UP))
            {
                Game.DesiredFPS += 0.5f;
                _desiredFPS.Text = $"Desired FPS: {Math.Round(Game.DesiredFPS, 2)}";

                Game.WindowHeight += 20;
            }


            if (Keyboard.WasKeyPressed(SDL.SDL_Keycode.SDLK_DOWN))
            {
                Game.DesiredFPS -= 0.5f;
                _desiredFPS.Text = $"Desired FPS: {Math.Round(Game.DesiredFPS, 2)}";

                Game.WindowHeight -= 20;
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
            //Render the start line
            SceneManager.SpriteBatch.Render(_startText, 135, 150);
            SceneManager.SpriteBatch.RenderLine(150, 175, 150, 275, Color.Green);

            //Render the finish line
            SceneManager.SpriteBatch.Render(_finishText, 335, 150);
            SceneManager.SpriteBatch.RenderLine(_finishLineX, 175, _finishLineX, 275, Color.Yellow);

            //Render the distance text
            SceneManager.SpriteBatch.Render(_distanceText, 250 - (_distanceText.Width / 2), 300);

            //Render the desired FPS text
            SceneManager.SpriteBatch.Render(_desiredFPS, 5, 5);

            //Render the current FPS
            SceneManager.SpriteBatch.Render(_currentFPS, 5, 20);

            //Render the type of game loop
            SceneManager.SpriteBatch.Render(_gameLoopType, 5, 35);

            //Render box face!!
            SceneManager.SpriteBatch.Render(_boxFace, (int)_boxFaceVel.X, (int)_boxFaceVel.Y);

            base.Render(elapsedTime);
        }
    }
}
