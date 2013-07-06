using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using OldSkull.GenericEntities;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    class IsleLevel : PlatformerLevel
    {
        public IsleLevel(PlatformerLevelLoader loader)
            : base((int)loader.size.X, (int)loader.size.Y)
        {
            loadLevel(loader);
        }

        public override void Begin()
        {
            base.Begin();
            Add(new TilableBackground("sky", -3));

            Player po = new Player(new Vector2(32), new Vector2(32));
            Add(po);
            CameraTarget = po;
        }
    }
}

