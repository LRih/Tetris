using System;
using System.Drawing;

namespace Engine
{
    public class Scene
    {
        //===================================================================== VARIABLES
        public readonly Window Container;

        //===================================================================== INITIALIZE
        public Scene(Window container)
        {
            Container = container;
        }
        //===================================================================== FUNCTIONS
        public virtual void Update()
        {
        }
        public virtual void Draw(Graphics g)
        {
        }
    }
}
