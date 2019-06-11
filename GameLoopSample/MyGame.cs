using GameLoopSample.Scenes;
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
        private SceneManager _sceneManager;
        private SpriteBatch _spriteBatch;


        public MyGame()
        {
            TimeStep = TimeStepType.Fixed;
        }


        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Renderer);

            _sceneManager = new SceneManager(_spriteBatch);
            _sceneManager.AddScene(new FPSScene());

            _sceneManager.Initialize();

            base.Initialize();
        }


        public override void Update(TimeSpan elapsedTime)
        {
            _sceneManager.Update(elapsedTime);

            base.Update(elapsedTime);
        }


        public override void Render(TimeSpan elapsedTime)
        {
            _sceneManager.Render(elapsedTime);

            base.Render(elapsedTime);
        }
    }
}
