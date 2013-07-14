using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Environment
{
    public class Npc : Entity
    {
        private bool Selected=false;
        private Sprite<string> image;

        private IsleLevel Level { get { return (IsleLevel)Scene; } }
        private string TalkDefault;

        public Npc(Vector2 Position, Collider Size, string Character ,string TalkDefault)
            : base(0)
        {
            this.Position = Position;
            this.TalkDefault = TalkDefault;

            image = OldSkullGame.SpriteData.GetSpriteString(Character);
            image.Play("stand");
            Add(image);
            Collider = Size;

            Tag(GameTags.Npc);
            Depth = 23;
        }

        public void Select()
        {
            Selected = true;
        }

        public override void Update()
        {
            base.Update();
            
        }

        public override void Render()
        {
            if (Selected) image.DrawFilledOutline(OldSkullGame.Color[3]);
            Selected = false;
            base.Render();
        }


        internal void onTalk(Player player)
        {
            Level.CurrentState = IsleLevel.GameState.Talk;

            if (TalkDefault.Contains("%Price"))
            {
                int price = int.Parse(TalkDefault.Remove(0, 6));
                Level.TalkBox.Start("Hey there stranger, are you interested in a ride? It's only " + price + (price==1?" coin":" coins") + ".");
                Level.TalkBox.Choice = Level.GoToMap;
            }
            else
            {
                Level.TalkBox.Start(TalkDefault);
            }
        }
    }
}
