using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Environment
{
    public class Door:Entity
    {
        private Sprite<string> Image;
        private bool Open;
        private IsleLevel Level { get { return (IsleLevel)Scene; } }
        private Rectangle KeyArea;
        private string Id;

        public Door(Vector2 Position, string Id)
            : base(0)
        {
            this.Position = Position;
            this.Id = Id;

            Image = OldSkullGame.SpriteData.GetSpriteString("door");
            Image.Play("stand");
            Tag(GameTags.Solid);
            Add(Image);
            Collider = new Hitbox(16, 32);
            KeyArea = new Rectangle((int)X - 2, (int)Y - 2, 20, 36);
        }

        public override void Update()
        {
            base.Update();


            if (!Open)
            {
                Drop Key = (Drop)Level.CollideFirst(KeyArea, GameTags.Drop);
                if (Key != null && Key.OpenDoors!=0)
                {
                    Image.Play("open");
                    Image.OnAnimationComplete = OnOpen;
                    Open = true;
                    Key.onOpen();
                }
            }
        }

        private void OnOpen(Sprite<string> Image)
        {
            Collidable = false;
            UserData.AffectDoor(Id, true);
        }
    }
}
