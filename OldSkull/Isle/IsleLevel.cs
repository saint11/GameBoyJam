using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
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
            Player po = new Player(new Vector2(20), new Vector2(20));
            po.Add(new Image(new Monocle.Texture(20, 20, Color.Brown)));
            Add(po);
            CameraTarget = po;
        }
    }
}

