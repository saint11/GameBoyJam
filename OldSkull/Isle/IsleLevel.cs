using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using OldSkull.Graphics;
using Monocle;
using Microsoft.Xna.Framework;
using System.Xml;

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
            Add(new TilableBackground("sky", -5));
        }

        public override void LoadEntity(XmlElement e)
        {
            if (e.Name == "Player")
            {
                Player po = new Player(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(13,24));
                Add(po);
                CameraTarget = po;
            }
        }

    }
}

