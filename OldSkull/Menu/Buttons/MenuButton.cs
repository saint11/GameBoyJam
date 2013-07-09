using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;

namespace OldSkull.Menu
{
    class MenuButton : Entity
    {

        private Action action;
        private Action DefaultFunction;
        public GraphicsComponent image { get; private set; }

        public MenuButton(GraphicsComponent image, Action action, Action DefaultFunction, int layer)
            :base(layer)
        {
            this.image = image;
            image.CenterOrigin();
            Add(image);
            this.action = action;
            this.DefaultFunction = DefaultFunction;
        }

        public void press()
        {
            if (action!=null) action();
            if (DefaultFunction != null) DefaultFunction();
        }

    }
}
