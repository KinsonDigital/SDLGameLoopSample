using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GameLoopSample.Scenes
{
    public class SceneManager
    {
        private readonly List<IScene> _scenes = new List<IScene>();


        public SceneManager(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;    
        }


        public static SpriteBatch SpriteBatch { get; private set; }


        public void AddScene(IScene scene)
        {
            //Deactivate all scenes
            _scenes.ForEach(s => s.IsActive = false);

            scene.IsActive = true;
            _scenes.Add(scene);
        }


        public void Initialize()
        {
            //Find active scene and initialize it
            var activeScenes = (from s in _scenes where s.IsActive select s).ToArray();

            if (activeScenes.Length > 0)
            {
                activeScenes[0].Initialize();
                return;
            }


            throw new Exception("Could not find active scene to initialize!.");
        }


        public void Update(TimeSpan elapsedTime)
        {
            foreach (var scene in _scenes)
            {
                if (scene.IsActive)
                    scene.Update(elapsedTime);
            }
        }


        public void Render(TimeSpan elapsedTime)
        {
            SpriteBatch.Begin();

            SpriteBatch.Clear(Color.FromArgb(255, 48, 48, 48));

            foreach (var scene in _scenes)
            {
                if (scene.IsActive)
                    scene.Render(elapsedTime);
            }

            SpriteBatch.End();
        }
    }
}
