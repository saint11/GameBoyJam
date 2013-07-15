using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using Microsoft.Xna.Framework;


namespace OldSkull.Isle.Environment
{
    class Coin:PlatformerObject
    {
        private bool Spinning=true;
        public Coin(Vector2 Position)
            : base(Position, new Vector2(8, 8))
        {
            image = OldSkullGame.SpriteData.GetSpriteString("coin");
            image.Play("coinSpin");
            Add(image);
            Speed.Y = -2;
            Speed.X = Calc.Random.NextFloat(3) - 1.5f;
            if (Speed.X < 0) image.Effects = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
        }

        public override void Update()
        {
            base.Update();
            if (Speed.Length() <= 0.05f & Spinning & onGround)
            {
                image.Play("coinGround");
                Spinning = false;
            }
            if (!Spinning)
            {
                Player Player = (Player)Scene.CollideFirst(Collider.Bounds, GameTags.Player);
                if (Player != null)
                {
                    OldSkullGame.Player.Coin++;
                    RemoveSelf();
                }
            }
        }

        protected override void onCollideH(Solid solid)
        {
            //base.onCollideH(solid);
            Speed.X *= -0.5f;
        }
        protected override void onCollideV(Solid solid)
        {
            //base.onCollideV(solid);
            if (Speed.Y > 0)
            {
                onGround = true;
            }
            Speed.Y *= -0.5f;
            if (Math.Abs(Speed.Y) < 0.1f) Speed.Y = 0;
        }

    }
}
