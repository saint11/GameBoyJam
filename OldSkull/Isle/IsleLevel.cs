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
        public Hud Hud;

        public IsleLevel(PlatformerLevelLoader loader)
            : base((int)loader.size.X, (int)loader.size.Y)
        {
            loadLevel(loader);
        }

        public override void Begin()
        {
            base.Begin();
            Add(new TilableBackground("sky", SKY_GAME_LAYER));

            Hud = new Hud();
            Add(Hud);

            skyGameLayer.CameraMultiplier = 0.8f;

        }

        public override void LoadEntity(XmlElement e)
        {
            if (e.Name == "Player")
            {
                Player po = new Player(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(13, 24), "jonathan");
                Add(po);
                CameraTarget = po;
            }
            else if (e.Name == "Fruit")
            {
                Add(new Drop(new Vector2(e.AttrFloat("x"), e.AttrFloat("y"))));
            }
            else if (e.Name == "SoftGround")
            {
                Add(new Container(new Vector2(e.AttrFloat("x"), e.AttrFloat("y"))));
            }
        }

        internal void showContext(Drop Holding, Player player)
        {
            Add(new Isle.ContextMenu(Holding,player));
        }

    }
}

