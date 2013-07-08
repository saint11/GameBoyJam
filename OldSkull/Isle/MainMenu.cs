using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using OldSkull.Menu;
using OldSkull.GameLevel;

namespace OldSkull.Isle
{
    class MainMenu : Menu.MainMenu
    {
        private Entity title = new Entity(1);
        public override void Begin()
        {
            base.Begin();

            //Tittle Animation
            Image titleImage = new Image(OldSkullGame.Atlas["title"]);
            title.Add(titleImage);
            title.X = Engine.Instance.Screen.Width / 2 - titleImage.Width / 2;
            title.Y = -titleImage.Height;
            Add(title);
            Tween.Position(title, new Vector2(title.X, 10), 100, Ease.BackOut, Tween.TweenMode.Oneshot);

            SelectorMenu menu = new SelectorMenu(new string[] { "NEW", "EXIT" }, new Action[] { newGame, exitGame }, SelectorMenuEffects.Scale, false);
            menu.X = Engine.Instance.Screen.Width / 2;
            menu.Y = Engine.Instance.Screen.Height/ 2;
            Add(menu);
        }

        public void newGame()
        {
            PlatformerLevelLoader loader = PlatformerLevelLoader.load();
            PlatformerLevel level = new IsleLevel(loader);
            OldSkullGame.Instance.Scene = level;
        }

        public void exitGame()
        {
            Engine.Instance.Exit();
        }
    }
}
