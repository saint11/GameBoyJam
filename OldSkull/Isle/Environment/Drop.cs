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
        public enum DropType { Fruit, Throwable };

        private Player HoldedBy;
        private bool Selected = false;
        public string Name { get; private set; }
        public int MatureTime { get; private set; }
        public int MaxLevel { get; private set; }
        public int FruitSpawn { get; private set; }

        private float ImpactDamage;
        private int Uses;

        public bool CanBePlanted { get; private set; }

        private PlayerStatEffect BodyEffect;
        private PlayerStatEffect SoulEffect;

        public DropType MyType { get; private set; }

        private bool Attacking { get { return (HoldedBy==null && Speed.Length() > 3); } }
        private IsleLevel Level { get { return (IsleLevel)Scene; } }

        public Drop(Vector2 position, string Name)
            : base(position+new Vector2(8), new Vector2(10))
        {
            this.Name = Name;
            
            GroundDamping.X = 0.9f;

            image = OldSkullGame.SpriteData.GetSpriteString("itens16");

            XmlDocument Xml = new XmlDocument();
            Xml.Load(OldSkullGame.Path + @"Content/Misc/Itens.xml");
            XmlElement XmlItem = Xml["Itens"][Name];

            BodyEffect = new PlayerStatEffect();
            SoulEffect = new PlayerStatEffect();

            switch (XmlItem.Attr("Type"))
            {
                case "Fruit": MyType = DropType.Fruit; break;
                case "Throwable": MyType = DropType.Throwable; break;
                default:
                    throw new Exception("Item does not have a type! Check your Itens.xml");
            }

            if (MyType == DropType.Fruit)
            {
                if (XmlItem.HasChild("Body"))
                {
                    BodyEffect.Duration = XmlItem["Body"].ChildInt("Duration", 0);
                    BodyEffect.Increment = XmlItem["Body"].ChildFloat("Increment", 0);
                }

                if (XmlItem.HasChild("Soul"))
                {
                    SoulEffect.Duration = XmlItem["Soul"].ChildInt("Duration", 0);
                    SoulEffect.Increment = XmlItem["Soul"].ChildFloat("Increment", 0);
                }

                MatureTime = 200;
                MaxLevel = 3;
                FruitSpawn = 3;
            }
            else if (MyType == DropType.Throwable)
            {
                ImpactDamage = XmlItem.ChildFloat("Damage");
                Uses = XmlItem.ChildInt("Hp");
            }

            image.Play(XmlItem.ChildText("Image"));
            

            Add(image);
            Depth = -10;
            Tag(GameTags.Drop);
        }

        public void onPickUp(Player player)
        {
            HoldedBy = player;
            Collidable = false;
            Depth = 10;
            if (Scene!=null) Scene.Layers[LayerIndex].DepthSortEntities(Layer.SortByDepth);
        }

        public override void Update()
        {
            base.Update();
            if (HoldedBy != null)
            {
                Position = HoldedBy.HandPosition;
                image.Effects = HoldedBy.image.Effects;
            }

            if (Attacking)
            {
                Environment.Enemy enemy = (Environment.Enemy)Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
                if (enemy!=null)
                {
                    enemy.TakeDamage(ImpactDamage,Position);
                    Uses--;
                    if (Uses == 0) onBreak();
                }
            }
        }

        private void onBreak()
        {
            Scene.Add(new Fx.Explosion(Position, LayerIndex));
            RemoveSelf();
        }

        internal void onDropped()
        {
            HoldedBy = null;
            Collidable = true;
            Depth = -10;
            Scene.Layers[LayerIndex].DepthSortEntities(Layer.SortByDepth);
        }

        internal void onUse(Player player)
        {
            if (MyType == DropType.Fruit)
            {
                if (!BodyEffect.Exausted) OldSkullGame.Player.AddBodyEffect(BodyEffect);
                if (!SoulEffect.Exausted) OldSkullGame.Player.AddSoulEffect(SoulEffect);
                player.Holding = null;
                RemoveSelf();
            }
            else if (MyType == DropType.Throwable)
            {
                Speed.X = player.side * 8;
                Speed.Y = -0.8f;
                onDropped();
                player.Holding = null;
            }
        }

        public string Action { get {
            if (MyType == DropType.Fruit) return "eat";
            else if (MyType == DropType.Throwable) return "throw";
            else return null;
        } }

        internal void onPlace()
        {
            RemoveSelf();
            HoldedBy = null;
        }

        internal void onSwitch()
        {
            
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
