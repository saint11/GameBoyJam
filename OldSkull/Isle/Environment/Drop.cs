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
        private string Id;

        public DropType MyType { get; private set; }

        private bool Attacking { get { return (HoldedBy==null && Speed.Length() > 3); } }
        private IsleLevel Level { get { return (IsleLevel)Scene; } }

        public static Vector2 POSITION_FIX = new Vector2(8);

        public Drop(Vector2 position, string Name, string Id)
            : base(position+POSITION_FIX, new Vector2(10))
        {
            this.Name = Name;
            this.Id = Id;
            
            GroundDamping.X = 0.9f;

            image = OldSkullGame.SpriteData.GetSpriteString("itens16");
            Add(image);
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
            Depth = -10;
            Tag(GameTags.Drop);
        }

        public void onPickUp(Player player)
        {
            HoldedBy = player;
            Collidable = false;
            Depth = 10;
            if (Scene != null)
            {
                Scene.Layers[LayerIndex].DepthSortEntities(Layer.SortByDepth);
                UserData.AffectItem(Id, Level.Name, null);
            }
        }

        public override void Update()
        {
            base.Update();
            if (!onGround)
            {
                UserData.AffectItem(Id, Level.Name,this);
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

            if (X < 0)
            {
                X = 0;
                if (Speed.X < 0) Speed.X *= -0.8f;
            }
            if (X > (int)Level.Width) 
            {
                X = Level.Width;
                if (Speed.X > 0) Speed.X *= -0.8f;
            }
        }

        private void onBreak()
        {
            UserData.AffectItem(Id, Level.Name, null);
            Scene.Add(new Fx.Explosion(Position, LayerIndex));
            RemoveSelf();
        }

        internal bool onDropped()
        {
            UserData.AffectItem(Id, Level.Name, this);
            if (!Level.CollideCheck(Collider.Bounds, GameTags.Solid))
            {
                HoldedBy = null;
                Collidable = true;
                Depth = -10;
                Scene.Layers[LayerIndex].DepthSortEntities(Layer.SortByDepth);
                return true;
            }
            return false;
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

        public override void SceneEnd()
        {
            base.SceneEnd();
            RemoveSelf();
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
            if (HoldedBy != null)
            {
                Position = HoldedBy.HandPosition;
                image.Effects = HoldedBy.image.Effects;
            }
            base.Render();

            Selected = false;
        }

        protected override void onCollideH(Solid solid)
        {
            //base.onCollideH(solid);
            Speed.X *= -0.25f;
        }
        protected override void onCollideV(Solid solid)
        {
            //base.onCollideV(solid);
            if (Speed.Y > 0)
            {
                onGround = true;
            }
            Speed.Y *= -0.3f;
            if (Math.Abs(Speed.Y) < 0.1f) Speed.Y = 0;
        }
    }
}
