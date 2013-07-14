using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull;
using Microsoft.Xna.Framework;


namespace OldSkull.Isle
{
    public class Hud : Entity
    {

        private Text text;
        private string currentAction;
        public string action;

        private Image Context;

        private Rectangle BodyBar;
        private Rectangle SoulBar;

        public Hud()
            : base(IsleLevel.HUD_LAYER)
        {
            Image image = new Image(OldSkullGame.Atlas["ui/meters"]);
            image.Position = new Vector2(2);
            Add(image);

            Context = new Image(OldSkullGame.Atlas["ui/contextItem"]);
            Context.Position = new Vector2(70, 4);
            Context.Visible = false;
            Add(Context);

            text = new Text(OldSkullGame.Font, "eat", new Vector2(90, 13), Text.HorizontalAlign.Left);
            text.Color = OldSkullGame.Color[2];
            Add(text);

            BodyBar = new Rectangle(24 + (int)image.X, 6 + (int)image.Y, 38, 2);
            SoulBar = new Rectangle(24 + (int)image.X, 14 + (int)image.Y, 38, 2);
        }

        public override void Update()
        {
            base.Update();
            if (action != currentAction)
            {
                text.DrawText = action;
                currentAction = action;
                Context.Visible = (action != "");
            }

            BodyBar.Width = (int)(38.0f * OldSkullGame.Player.Body);
            SoulBar.Width = (int)(38.0f * OldSkullGame.Player.Soul);
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(BodyBar, OldSkullGame.Color[3]);
            Draw.Rect(SoulBar, OldSkullGame.Color[3]);
        }
    }
}
