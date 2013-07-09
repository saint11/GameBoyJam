using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;
using System.Xml;

namespace OldSkull.Isle
{
    public class Drop : PlatformerObject
    {
        private Player HoldedBy;
        private bool Selected = false;
        public int MatureTime { get; private set; }
        public int MaxLevel { get; private set; }
        public int FruitSpawn { get; private set; }

        public bool CanBePlanted { get; private set; }

        private PlayerStatEffect BodyEffect;
        private PlayerStatEffect SoulEffect;

        public Drop(Vector2 position)
            : base(position+new Vector2(8), new Vector2(10))
        {
            GroundDamping.X = 0.9f;

            image = OldSkullGame.SpriteData.GetSpriteString("items16");

            XmlDocument Xml = new XmlDocument();
            Xml.Load(OldSkullGame.Path + @"Content/Misc/Itens.xml");
            XmlElement XmlItem = Xml["Itens"]["Apple"];

            BodyEffect = new PlayerStatEffect();
            BodyEffect.Duration = XmlItem["Body"].ChildInt("Duration",0);
            BodyEffect.Increment = XmlItem["Body"].ChildFloat("Increment",0);

            SoulEffect = new PlayerStatEffect();
            SoulEffect.Duration = XmlItem["Soul"].ChildInt("Duration",0);
            SoulEffect.Increment = XmlItem["Soul"].ChildFloat("Increment",0);


            image.Play(XmlItem.ChildText("Image"));
            MatureTime = 200;
            MaxLevel = 3;
            FruitSpawn = 3;

            Add(image);

            Tag(GameTags.Drop);
        }

        public void onPickUp(Player player)
        {
            HoldedBy = player;
            Collidable = false;
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
            Collidable = true;
        }

        internal void onUse(Player player)
        {
            if (!BodyEffect.Exausted) OldSkullGame.Player.AddBodyEffect(BodyEffect);
            if (!SoulEffect.Exausted) OldSkullGame.Player.AddSoulEffect(SoulEffect);
            player.Holding = null;
            RemoveSelf();
        }

        public string Action { get { return "eat"; } }

        internal void onPlace()
        {
            HoldedBy = null;
        }

        public string ContextPlace { get { return "PLANT"; } }

        internal void Select()
        {
            Selected = true;
        }

        public override void Render()
        {
            if (Selected) image.DrawFilledOutline(OldSkullGame.Color[3]);
            base.Render();

            Selected = false;
        }
    }
}
