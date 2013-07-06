﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    class PlatformerObject : Entity
    {
        //This entity should be abble to detect and act acordingly with
        //all kinds of Collision.
        //Ideally we want this kind of reaction to be turned on or off.

        public Vector2 Speed;
        public Vector2 MaxSpeed;
        public Vector2 Gravity;
        private Vector2 counter;

        private PlatformerLevel Level;
        private Boolean onGround = false;

        public PlatformerObject(Vector2 position, Vector2 size)
            :base(PlatformerLevel.GAMEPLAY_LAYER)
        {
            Position = position;
            Collider = new Hitbox(size.X, size.Y);
            Speed = new Vector2();
            MaxSpeed = new Vector2(5);
        }
        public override void Added()
        {
            base.Added();

            //TODO: Check if its on a platformerLevel
            Level = (PlatformerLevel)Scene;
            Gravity = Level.Gravity;
        }

        public override void Update()
        {
            base.Update();
            Speed += Gravity;

            LimitMaxSpeed();
            Move(Speed, onCollideH, onCollideV);
        }

        private void LimitMaxSpeed()
        {
            if (Math.Abs(Speed.X) > MaxSpeed.X) Speed.X = MaxSpeed.X;
            if (Math.Abs(Speed.Y) > MaxSpeed.Y) Speed.Y = MaxSpeed.Y;
        }

        private void onCollideH(Solid solid)
        {
            Speed.X = 0;
        }
        private void onCollideV(Solid solid)
        {
            if (Speed.Y > 0)
            {
                onGround = true;
            }

            Speed.Y = 0;
        }

        public void MoveH(float moveH, Action<Solid> onCollide = null)
        {
            counter.X += moveH;
            int move = (int)Math.Round(counter.X);

            if (move != 0)
            {
                Entity hit;
                int sign = Math.Sign(move);
                counter.X -= move;

                while (move != 0)
                {
                    if ((hit = CollideFirst(GameTags.Solid, X + sign, Y)) != null)
                    {
                        counter.X = 0;
                        if (onCollide != null)
                            onCollide(hit as Solid);
                        break;
                    }

                    X += sign;
                    move -= sign;
                }
            }
        }

        public void MoveV(float moveV, Action<Solid> onCollide = null)
        {
            counter.Y += moveV;
            int move = (int)Math.Round(counter.Y);
            onGround = false;

            if (move != 0)
            {
                Entity hit;
                int sign = Math.Sign(move);
                counter.Y -= move;

                while (move != 0)
                {
                    if ((hit = CollideFirst(GameTags.Solid, X, Y + sign)) != null)
                    {
                        counter.Y = 0;
                        if (onCollide != null)
                            onCollide(hit as Solid);
                        break;
                    }

                    Y += sign;
                    move -= sign;
                }
            }
        }

        public void Move(Vector2 amount, Action<Solid> onCollideH = null, Action<Solid> onCollideV = null)
        {
            MoveH(amount.X, onCollideH);
            MoveV(amount.Y, onCollideV);
        }
        private Boolean willCollide(Vector2 Position)
        {
            Boolean collided = false;
            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, (int)Collider.Width, (int)Collider.Height);
            foreach (Entity e in Level.Solids)
            {
                if (e.Collider.Collide(rect))
                {
                    collided = true;
                    break;
                }
            }
            return collided;
        }

        private Boolean ComparePosition(Vector2 vec1, Vector2 vec2)
        {
            const float threshold = 0.1f;
            Boolean sameX = Math.Abs(vec1.X - vec2.X) <= threshold;
            Boolean sameY = Math.Abs(vec1.Y - vec2.Y) <= threshold;
            return (sameX && sameY);
        }
    }
}
