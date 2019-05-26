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
        private Texture _boxFace;
        private SpriteBatch _spriteBatch;


        public MyGame() : base(640, 480)
        {
            var myTimer = new Stopwatch();
            myTimer.Start();
            myTimer.Stop();
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Renderer);

            _gameText = new GameText(Renderer, "OpenSans-Regular.ttf", string.Empty, 12, Color.White);

            _boxFace = new Texture(Renderer, "OrangeBox")
            {
                X = 200,
                Y = 200
            };

            base.Initialize();
        }


        public override void Update()
        {
            _gameText.Text = $"FPS: {FPS}";

            if (Keyboard.IsKeyDown(SDL.SDL_Keycode.SDLK_RIGHT))
            {
                _boxFace.X += 1;
            }

            base.Update();
        }


        public override void Render()
        {
            _spriteBatch.Begin();

            _spriteBatch.Clear(Color.FromArgb(255, 48, 48, 48));

            _spriteBatch.Render(_boxFace, 200, 200);
            _spriteBatch.Render(_gameText, 5, 5);

            _spriteBatch.End();

            base.Render();
        }
    }
}
