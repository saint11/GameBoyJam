﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class Container :Entity
    {
        private bool Selected = false;
        private Sprite<string> image;
        private Drop Stored;

        private int Lifetime=0;
        private int Level = -1;
        public int Hp = 3;
        public Drop.DropType DropType;
        public bool CanHarvest {get;private set;}

        private string Id;
        private int lastSeen = 0;

        public Container(Vector2 Position, int Hp, string Id)
            : base(IsleLevel.BG_GAME_LAYER)
        {
            Tag(GameTags.Container);
            this.Position = Position;
            this.Hp = Hp;
            this.Id = Id.ToUpper();

            image = OldSkullGame.SpriteData.GetSpriteString("plants");
            image.Play("softGround");
            Add(image);

            Collider = new Hitbox(16, 32);
            CanHarvest = false;

            DropType = Drop.DropType.Fruit;

            if (Level > 0)
                image.Play(Stored.Name + Level);
        }

        internal void Select()
        {
            Selected = true;
        }
        public override void Added()
        {
            base.Added();
            if (Hp == 0)
            {
                RemoveSelf();
            }
            else
            {
                SimulateTime(OldSkullGame.GetTotalTime() - lastSeen);
            }
        }
        public override void Update()
        {
            base.Update();

            if (!Empty)
            {
                Lifetime++;
                if (Lifetime > Stored.MatureTime)
                {
                    Upgrade();
                }
            }
            lastSeen = OldSkullGame.GetTotalTime();
        }

        private void Upgrade()
        {
            if (Level < Stored.MaxLevel)
            {
                Level++;
                Lifetime = 0;
                image.Play(Stored.Name + Level);
                if (Level == Stored.MaxLevel) CanHarvest = true;
            }

            UserData.AffectGround(Id, this);
        }


        public override void Render()
        {
            if (Selected) image.DrawFilledOutline(OldSkullGame.Color[3]);
            base.Render();
            
            Selected = false;
        }

        internal void Place(Drop Placed)
        {
            Stored = Placed;
            UserData.AffectItem(Placed.Id, ((IsleLevel)Scene).Name, null, Placed.KeyItem);

            image.Play(Stored.Name+"0");
            Lifetime = 0;
        }

        public void SimulateTime(int time)
        {
            int TotalLifetime = time;

            if (Stored != null)
            {
                for (int i = 0; i < time / Stored.MatureTime; i++)
                {
                    Upgrade();
                    TotalLifetime -= Stored.MatureTime;
                }
                Lifetime = TotalLifetime;

                image.Play(Stored.Name + Level);
            }
        }

        public bool Empty { get { return Stored == null; } }

        public string Action { get { return Empty? "plant" : CanHarvest? "harvest":""; } }

        public override void SceneEnd()
        {
            base.SceneEnd();
            RemoveSelf();
        }

        internal void Harvest()
        {
            image.Play("softGround");
            Lifetime = 0;
            Level = 0;

            for (int i = 0; i < Stored.FruitSpawn; i++)
            {
                Drop d = new Drop(Position, Stored.Name,"DYNAMIC"+UserData.DynamicItems.ToString());
                UserData.DynamicItems++;
                Scene.Add(d);
                d.Speed.Y = -1.5f;
                d.Speed.X = 1 - Calc.Random.NextFloat(2);
            }

            Stored = null;
            CanHarvest = false;

            Hp--;

            UserData.AffectGround(Id, this);
            if (Hp == 0)
            {
                RemoveSelf();
            }

        }
    }
}
