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


        public MyGame() : base(640, 480)
        {
            var myTimer = new Stopwatch();
            myTimer.Start();
            myTimer.Stop();
        }


        public override void Initialize()
        {
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
            SDL.SDL_RenderClear(Renderer);

            _boxFace.Render();
            _gameText.Render();

            SDL.SDL_RenderPresent(Renderer);

            base.Render();
        }
    }
}
