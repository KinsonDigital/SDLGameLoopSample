using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample.Scenes
{
    public interface IScene
    {
        bool IsActive { get; set; }

        void Initialize();


        void Update(TimeSpan elapsedTime);


        void Render(TimeSpan elapsedTime);
    }
}
