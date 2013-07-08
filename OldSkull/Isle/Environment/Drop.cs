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
        public int MatureTime { get; private set; }
        public int MaxLevel { get; private set; }
        public int FruitSpawn { get; private set; }

        public bool CanBePlanted { get; private set; }

        public Drop(Vector2 position)
            : base(position+new Vector2(8), new Vector2(16))
        {
            GroundDamping.X = 0.9f;

            image = OldSkullGame.SpriteData.GetSpriteString("items16");

            image.Play("apple");
            MatureTime = 200;
            MaxLevel = 3;
            FruitSpawn = 3;

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
            Speed.X = HoldedBy.Speed.X +Calc.Random.NextFloat(2) - 1;
            Speed.Y = -1;
            HoldedBy = null;
        }

        internal void onUse(Player player)
        {
            player.Holding = null;
            RemoveSelf();
        }

        public string Action { get { return "eat"; } }

        internal void onPlace()
        {
            HoldedBy = null;
        }
    }
}
