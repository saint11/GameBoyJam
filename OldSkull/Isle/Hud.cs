﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull;
using Microsoft.Xna.Framework;


namespace OldSkull.Isle
{
    class Hud : Entity
    {

        private Text text;
        private string currentAction;
        public string action;

        public Hud()
            : base(IsleLevel.HUD_LAYER)
        {
            Image image = new Image(OldSkullGame.Atlas["ui/meters"]);
            image.Position = new Vector2(2);
            Add(image);

            text = new Text(OldSkullGame.Font, "eat", new Vector2(93, 13), Text.HorizontalAlign.Left);
            text.Color = OldSkullGame.Color[2];
            Add(text);

        }

        public override void Update()
        {
            base.Update();
            if (action != currentAction)
            {
                text.DrawText = action;
                currentAction = action;
            }
        }

    }
}
