﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    class ContextMenu : Entity
    {
        private Drop Holding;
        private List<Entity> childs;
        private Menu.SelectorMenu menu;
        private GameLevel.Player player;

        public ContextMenu(Drop Holding, GameLevel.Player player)
            : base(IsleLevel.HUD_LAYER)
        {
            this.Holding = Holding;
            this.player = player;

            childs = new List<Entity>();
        }

        public override void Added()
        {
            base.Added();
            Menu.Effect effect = new Menu.Effect(10, 0.85f, 1.2f, Menu.SelectorMenuEffects.ColorIn, Menu.SelectorMenuEffects.ColorOut);
            effect.outline = Color.Black;
            effect.selectedColor = OldSkullGame.Color[2];
            effect.deselectedColor = OldSkullGame.Color[0];

            menu = new Menu.SelectorMenu(new string[] { "EAT", Holding.ContextPlace, "DROP", "MIX", "CANCEL" }, new Action[] { onEat, onPlace, onDrop, onMix, onCancel }, effect, false, IsleLevel.HUD_LAYER);
            menu.hAlign="left";
            menu.X = 8;
            menu.Y = 28;
            menu.setColor(new Color(252, 235, 229));
            childs.Add(menu);

            Image image = new Image(OldSkullGame.Atlas["ui/popUpMenu"]);
            image.X = 2;
            image.Y = 25;
            Add(image);

            Scene.Add(menu);
        }

        private void onEat()
        {
            Holding.onUse(player);
            RemoveSelf();
        }

        private void onPlace()
        {
            player.PlaceItem();
            RemoveSelf();
        }

        private void onMix()
        {
            RemoveSelf();
        }
        private void onDrop()
        {
            player.dropItem();
            RemoveSelf();
        }
        private void onCancel()
        {
            RemoveSelf();
        }

        public override void Removed()
        {
            base.Removed();
            foreach (Entity e in childs)
            {
                e.RemoveSelf();
            }
            menu.RemoveSelf();
            player.stopUsing();
        }
    }
}
