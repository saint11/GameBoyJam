using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;


namespace OldSkull.Isle
{
    class WorldMap:Scene
    {
        private int from;
        private Layer skyGameLayer;
        private Layer bgGameLayer;

        public WorldMap(int from)
            :base()
        {
            this.from = from;
            
            SetLayer(-1, skyGameLayer = new Layer());
            SetLayer(0, bgGameLayer = new Layer());
            
            Entity bg = new Entity(0);
            bg.Add(new Image(OldSkullGame.Atlas["map/base"]));
            Add(bg);
        }
        

    }
}
