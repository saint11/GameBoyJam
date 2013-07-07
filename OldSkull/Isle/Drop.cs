using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    class Drop : PlatformerObject
    {
        public Drop(Vector2 position)
            : base(position, new Vector2(16))
        {
            image = OldSkullGame.SpriteData.GetSpriteString("items16");
            image.Play("apple");
            Add(image);

            Speed.X = Calc.Random.NextFloat(2)-1;
            //Speed.X = Calc.Random.Range(-2, 2);
            Speed.Y = -1;
        }

    }
}
