using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    class Drop : PlatformerObject
    {
        private Player HoldedBy;

        public Drop(Vector2 position)
            : base(position+new Vector2(8), new Vector2(16))
        {
            image = OldSkullGame.SpriteData.GetSpriteString("items16");
            image.Play("apple");
            Add(image);

            Tag(GameTags.Drop);
        }

        public void onPickUp(Player player)
        {
            HoldedBy = player;
        }

        public override void Update()
        {
            base.Update();
            if (HoldedBy != null)
            {
                Position = HoldedBy.HandPosition;
            }
        }

        internal void onDropped()
        {
            HoldedBy = null;
            Speed.X = Calc.Random.NextFloat(2) - 1;
            Speed.Y = -1;
        }

        internal void onUse(Player player)
        {
            player.Holding = null;
            RemoveSelf();
        }
    }
}
