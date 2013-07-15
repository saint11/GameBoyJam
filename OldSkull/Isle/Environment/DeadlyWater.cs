using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Environment
{
    public class DeadlyWater:Entity
    {
        public DeadlyWater(Vector2 Position, Vector2 Size)
            : base(1)
        {
            this.Position = Position;
            Collider = new Hitbox(Size.X, Size.Y);
            Image Image = new Image(new Texture((int)Size.X, (int)Size.Y, OldSkullGame.Color[1]));
            Add(Image);
        }
        public override void Update()
        {
            base.Update();
            GameLevel.Player Player = (GameLevel.Player)Scene.CollideFirst(Collider.Bounds, GameTags.Player);
            if (Player != null)
            {
                Player.onDrowning();
            }
        }
    }
}
