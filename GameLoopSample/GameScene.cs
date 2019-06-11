using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public class GameScene
    {
        public bool IsActive { get; set; }


        public virtual void Initialize()
        {

        }


        public virtual void Update(TimeSpan elapsedTime)
        {

        }


        public virtual void Render(TimeSpan elapsedTime)
        {

        }
    }
}
