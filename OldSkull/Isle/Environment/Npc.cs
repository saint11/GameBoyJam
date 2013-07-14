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
        private string TalkComplete;
        private string Wants;
        private string Reward;

        public Npc(Vector2 Position, Collider Size, string Character ,string TalkDefault, string Wants, string TalkComplete, string Reward)
            : base(0)
        {
            this.Position = Position;
            this.TalkDefault = TalkDefault;
            this.Wants = Wants;
            this.Reward = Reward;
            this.TalkComplete = TalkComplete;

            image = OldSkullGame.SpriteData.GetSpriteString(Character);
            image.Play("stand");
            Add(image);
            Collider = Size;
            Collider.Position.X -= 16;
            Collider.Position.Y -= 16;

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

            if (player.Holding != null && player.Holding.Name==Wants)
            {
                player.Holding.onPlace();
                player.Holding = null;
                Level.TalkBox.Start(TalkComplete);
                GetReward();
            }
            else if (TalkDefault.Contains("%Price"))
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

        private void GetReward()
        {
            switch (Reward)
            {
                case "apple":
                    Scene.Add(new Drop(Position,"apple","DYNAMIC"+UserData.DynamicItems)); break;
                case "WinGame":
                    Scene.End();
                    Engine.Instance.Scene = new GameOver(); break;
                default:
                    break;
            }
            UserData.DynamicItems++;
        }
    }
}
