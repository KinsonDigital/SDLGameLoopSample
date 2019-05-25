using SDL2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace GameLoopSample
{
    public class MyGame : Game
    {
        private static GameText _gameText;


        public MyGame() : base(640, 480)
        {
        }


        public override void Initialize()
        {
            _gameText = new GameText(Renderer, "OpenSans-Regular.ttf", "Hello World", 25, Color.White);

            base.Initialize();
        }


        public override void Update()
        {
            _gameText.Text = DateTime.Now.ToLongTimeString();

            base.Update();
        }


        public override void Render()
        {
            SDL.SDL_RenderClear(Renderer);

            _gameText.Render();

            SDL.SDL_RenderPresent(Renderer);

            base.Render();
        }
    }
}
