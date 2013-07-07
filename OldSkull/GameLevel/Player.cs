using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    class Player : PlatformerObject
    {
        private string imageName;
        private PlatformerLevel Level;
        public Isle.Drop Holding;
        private int side=1;

        public Player(Vector2 position, Vector2 size,string imageName)
            : base(position + size/2, size)
        {
            this.imageName = imageName;
            AirDamping.X = 0.9f;
            GroundDamping.X = 0.9f;
        }

        public override void Update()
        {
            base.Update();

            UpdateControls();
        }

        private void UpdateControls()
        {
            if (Math.Abs(KeyboardInput.xAxis) > 0)
            {
                Speed.X += KeyboardInput.xAxis * 0.2f;
                if (KeyboardInput.xAxis < 0)
                {
                    side = -1;
                    image.FlipX = true;
                }
                else
                {
                    image.FlipX = false;
                    side = 1;
                }

                image.Play("walk");
            }
            else
            {
                image.Play("idle");
                Speed.X *= 0.9f;
            }

            if (KeyboardInput.pressedInput("jump"))
            {
                if (onGround) Speed.Y = -4;
            }
            else if (!KeyboardInput.checkInput("jump") && (Speed.Y<0))
            {
                Speed.Y *= 0.7f;
            }


            if (KeyboardInput.pressedInput("use"))
            {
                if (Holding != null)
                {
                    Holding.onUse(this);
                }
            }
            if (KeyboardInput.pressedInput("down"))
            {
                if (Holding == null)
                {
                    Isle.Drop e = (Isle.Drop)Level.CollideFirst(Collider.Bounds, GameTags.Drop);
                    if (e != null)
                    {
                        e.onPickUp(this);
                        Holding = e;
                    }
                }
                else
                {
                    Holding.onDropped();
                    Holding = null;
                }   
            }
        }

        public override void Added()
        {
            base.Added();

            MaxSpeed = new Vector2(2f,3);
            image = OldSkullGame.SpriteData.GetSpriteString(imageName);
            Add(image);
            //image.CenterOrigin();
            image.Play("idle", true);
            Level = (PlatformerLevel)Scene;
        }

        public Vector2 HandPosition { get { return new Vector2(Position.X+8*side,Position.Y-1); } }
    }
}
