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
        private Action<int> DefaultFunction;
        private int Index;
        public GraphicsComponent image { get; private set; }

        public MenuButton(GraphicsComponent image, Action action, Action<int> DefaultFunction, int Index, int layer)
            :base(layer)
        {
            this.image = image;
            this.Index = Index;
            image.CenterOrigin();
            Add(image);
            this.action = action;
            this.DefaultFunction = DefaultFunction;
        }

        public void press()
        {
            if (action!=null) action();
            if (DefaultFunction != null) DefaultFunction(Index);
        }

    }
}
