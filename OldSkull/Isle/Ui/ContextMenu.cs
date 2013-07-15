using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using OldSkull.Menu;

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

            string[] actionNames;
            Action<MenuButton>[] actions;
            if (Holding!=null)
            {
                actionNames = new string[] { "EAT", Holding.ContextPlace, "DROP", "MIX", "CANCEL" };
                actions = new Action<MenuButton>[] { onEat, onPlace, onDrop, onMix, onCancel };
            }
            else
            {
                actionNames = new string[] {"PICK", "HARV.", "CANCEL" };
                actions = new Action<MenuButton>[] { onPick, onHarv, onCancel };
            }
            menu = new Menu.SelectorMenu(actionNames, actions, null, effect, false, IsleLevel.HUD_LAYER);
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

        private void onEat(MenuButton Mb)
        {
            Holding.onUse(player);
            RemoveSelf();
        }

        private void onPlace(MenuButton Mb)
        {
            player.InteractContainer(false);
            RemoveSelf();
        }

        private void onMix(MenuButton Mb)
        {
            RemoveSelf();
        }
        private void onDrop(MenuButton Mb)
        {
            player.dropItem();
            RemoveSelf();
        }
        private void onCancel(MenuButton Mb)
        {
            RemoveSelf();
        }
        private void onHarv(MenuButton Mb)
        {
            player.DefaultUse();
            RemoveSelf();
        }
        private void onPick(MenuButton Mb)
        {
            player.onPickUp(player.SelectedDrop);
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
