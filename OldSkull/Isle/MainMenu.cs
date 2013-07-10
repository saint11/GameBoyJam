﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Monocle;
using OldSkull.Menu;
using OldSkull.GameLevel;

namespace OldSkull.Isle
{
    class MainMenu : Menu.MainMenu
    {
        private Entity title = new Entity(1);
        SelectorMenu menu;
        public override void Begin()
        {
            base.Begin();
            Engine.Instance.Screen.ClearColor = OldSkullGame.Color[2];

            //Tittle Animation
            Image titleImage = new Image(OldSkullGame.Atlas["title"]);
            title.Add(titleImage);
            title.X = Engine.Instance.Screen.Width / 2 - titleImage.Width / 2;
            title.Y = -titleImage.Height;
            Add(title);
            Tween.Position(title, new Vector2(title.X, -5), 30, Ease.BackOut, Tween.TweenMode.Oneshot);

            Effect effect = new Effect(10, 0.85f, 1.2f, SelectorMenuEffects.ColorIn, SelectorMenuEffects.ColorOut);
            effect.outline = Color.Black;
            effect.selectedColor = OldSkullGame.Color[3];
            effect.deselectedColor = OldSkullGame.Color[0];

            menu = new SelectorMenu(new string[] { "NEW", "EXIT GAME" }, new Action[] { newGame, exitGame }, null, effect, false,1);
            menu.X = Engine.Instance.Screen.Width / 2;
            Add(menu);
        }

        public void newGame()
        {
            OldSkullGame.Player = new PlayerStats(null);
            PlatformerLevelLoader loader = PlatformerLevelLoader.load("grove");
            PlatformerLevel level = new IsleLevel(loader, IsleLevel.Side.Left,0);
            OldSkullGame.Instance.Scene = level;
        }

        public void exitGame()
        {
            Engine.Instance.Exit();
        }

        public override void Render()
        {
            menu.Y = title.Y + 98;
            base.Render();
        }
    }
}
