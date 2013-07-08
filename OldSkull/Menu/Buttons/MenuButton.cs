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
        public GraphicsComponent image { get; private set; }

        public MenuButton(GraphicsComponent image, Action action,int layer)
            :base(layer)
        {
            this.image = image;
            image.CenterOrigin();
            Add(image);
            this.action = action;
        }

        public void press()
        {
            if (action!=null) action();
        }

    }
}
