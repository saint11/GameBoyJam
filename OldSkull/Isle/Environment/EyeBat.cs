using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace OldSkull.Isle.Environment
{
    public class EyeBat:Entity
    {
        private List<Vector2> Nodes;
        private int CurrentNode=0;
        private Sprite<string> image;
        public float Damage = 0.1f;
        private int Invulnerable = 0;
        private float Hp = 0.5f;

        public EyeBat(XmlElement Xml)
            : base(0)
        {
            Tag(GameTags.Enemy);

            image = OldSkullGame.SpriteData.GetSpriteString("eyebat");
            image.Play("fly");
            image.Position += new Vector2(16);
            Add(image);

            Position = new Vector2(Xml.AttrFloat("x"), Xml.AttrFloat("y"));
            Nodes=new List<Vector2>();
            Nodes.Add(Position);

            Collider = new Hitbox(16, 16,8,8);

            foreach (XmlElement n in Xml.ChildNodes)
            {
                Nodes.Add(new Vector2(n.AttrFloat("x"), n.AttrFloat("y")));
            }
        }

        public override void Update()
        {
            base.Update();
            Invulnerable--;
            int NextNode=CurrentNode+1;
            if (NextNode>=Nodes.Count) NextNode=0;
            Vector2 Speed = Nodes[NextNode] - Position;
            Speed.Normalize();

            Position += Speed;
            image.Effects = Speed.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Vector2.Distance(Position, Nodes[NextNode]) < 2) CurrentNode=NextNode;
            if (Vector2.Distance(Position, Level.player.Position) < 70) Level.player.AddSoul(-0.0003f);
        }

        public void TakeDamage(float damage, Vector2 source)
        {
            if (Invulnerable <= 0)
            {
                Hp -= damage;
                Invulnerable = 50;
            }

            if (Hp <= 0)
            {
                onDeath();
                Level.player.AddSoul(0.1f);
            }
        }

        private void onDeath()
        {
            Level.Add(new Fx.Explosion(Position+new Vector2(16), LayerIndex));
            RemoveSelf();
        }
        private IsleLevel Level { get { return (IsleLevel)Scene; } }

        public override void Render()
        {
            if (Invulnerable > 0 && Invulnerable % 10 < 5)
            {
                image.RenderFilled(OldSkullGame.Color[2]);
            }
            else
            {
                base.Render();
            }
        }
    }
}
