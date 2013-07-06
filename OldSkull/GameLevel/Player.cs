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
        public Player(Vector2 position, Vector2 size)
            :base(position,size)
        {

        }

        public override void Update()
        {
            base.Update();

            Speed.X *= 0.8f;
            Speed.X += KeyboardInput.xAxis * 0.5f;

            if (KeyboardInput.pressedInput("jump"))
            {
                if (onGround) Speed.Y = -3;
            }
        }

        public override void Added()
        {
            base.Added();

            image = OldSkullGame.SpriteData.GetSpriteString("jonathan");
            Add(image);
            image.X = image.Width / 2;
            image.Y = image.Height / 2;
            image.Play("idle", true);
        }
    }
}
