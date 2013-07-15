using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;

namespace OldSkull.Isle
{
    class GameOver : OldSkull.Menu.MainMenu
    {
        public GameOver()
            :base()
        {

        }

        public override void Begin()
        {
            base.Begin();
            Entity Image = new Entity(0);
            Image.Add(new Image(OldSkullGame.Atlas["gameOver"]));
            Add(Image);
        }
        public override void Update()
        {
            base.Update();
            if (KeyboardInput.pressedInput("use") || KeyboardInput.pressedInput("jump"))
            {

                MainMenu.StartNewGame(false);
            }
        }
    }
}
