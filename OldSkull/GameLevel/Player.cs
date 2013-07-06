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

        public Player(Vector2 position, Vector2 size,string imageName)
            :base(position,size)
        {
            this.imageName = imageName;
        }

        public override void Update()
        {
            base.Update();

            UpdateControls();
        }

        private void UpdateControls()
        {
            Speed.X *= 0.8f;
            if (Math.Abs(KeyboardInput.xAxis) > 0)
            {
                Speed.X += KeyboardInput.xAxis * 0.2f;
                if (KeyboardInput.xAxis < 0) image.FlipX = true;
                else image.FlipX = false;

                image.Play("walk");
            }
            else
            {
                image.Play("idle");
            }

            if (KeyboardInput.pressedInput("jump"))
            {
                if (onGround) Speed.Y = -4;
            }
            else if (!KeyboardInput.checkInput("jump") && (Speed.Y<0))
            {
                Speed.Y *= 0.7f;
            }
        }

        public override void Added()
        {
            base.Added();

            MaxSpeed = new Vector2(2f,3);
            image = OldSkullGame.SpriteData.GetSpriteString(imageName);
            Add(image);
            image.X = image.Width / 2;
            image.Y = image.Height / 2;
            image.Play("idle", true);
        }
    }
}
