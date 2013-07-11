using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Fx
{
    class Explosion:Entity
    {
        public Explosion(Vector2 Position, int Layer)
            : base(Layer)
        {
            this.Position = Position;
            Sprite<int> image = OldSkullGame.SpriteData.GetSpriteInt("explosion");
            image.Play(0);
            Add(image);
            image.OnAnimationComplete = (Sprite<int> sprite) => { RemoveSelf(); };
        }
    }
}
