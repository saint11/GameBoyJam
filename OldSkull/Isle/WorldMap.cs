using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Map
{
    class WorldMap:Scene
    {
        private int from;
        private Layer skyGameLayer;
        private Layer bgGameLayer;

        private List<Node> nodes;
        public IsleLevel.Side lastPressed;
        public int Selected = 0;

        public WorldMap(int from, IsleLevel.Side direction)
            :base()
        {
            this.from = from;
            Selected = from;
            lastPressed = direction;
            SetLayer(-1, skyGameLayer = new Layer());
            SetLayer(0, bgGameLayer = new Layer());
            
            Entity bg = new Entity(0);
            bg.Add(new Image(OldSkullGame.Atlas["map/base"]));
            Add(bg);

            nodes = new List<Node>();

            XmlDocument xml = new XmlDocument();
            xml.Load(OldSkullGame.Path + @"Content\Misc\Map.xml");
            foreach (XmlElement n in xml["Map"])
            {
                Map.Node node = new Node(new Vector2(n["Position"].AttrFloat("X", 0), n["Position"].AttrFloat("Y", 0)),
                    n.Attr("Name"), n.ChildText("Level"),
                    n.ChildText("Up", ""), n.ChildText("Down", ""), n.ChildText("Left", ""), n.ChildText("Right", ""));

                Add(node);
                nodes.Add(node);
            }
            nodes[from].Select();
        }

        public override void Update()
        {
            base.Update();
            KeyboardInput.Update();
        }

        internal bool Select(string name)
        {
            if (name == "") return false;
            int i = 0;
            foreach (Node n in nodes)
            {
                if (n.Name == name)
                {
                    n.Select();
                    Selected = i;
                }
                else
                {
                    n.Deselect();
                }
                i++;
            }
            return true;
        }
    }
}
