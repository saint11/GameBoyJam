﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    public class PlatformerLevelLoader
    {
        public List<Solid> solids;
        public Vector2 size;

        public static PlatformerLevelLoader load()
        {
            PlatformerLevelLoader current = new PlatformerLevelLoader();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(OldSkullGame.Path + @"Content\Level\1.oel");
            XmlElement levelMap = xmlDoc["level"];

            current.size = new Vector2(int.Parse(levelMap.Attr("width")), int.Parse(levelMap.Attr("height")));

            current.solids = new List<Solid>();
            foreach (XmlElement e in levelMap["Solid"])
            {
                current.solids.Add(new Environment.Wall(int.Parse(e.Attr("x")), int.Parse(e.Attr("y")), int.Parse(e.Attr("w")), int.Parse(e.Attr("h"))));
            }

            return current;
        }
    }
}