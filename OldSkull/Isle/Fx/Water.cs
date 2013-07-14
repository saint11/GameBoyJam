using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle.Fx
{
    public class Water : Entity
    {
        private List<Sprite<int>> images;
        private float Shake;
        private float Speed;
        private bool WasOnWater = false;
        private float LastX;

        public Water(Vector2 Position, Vector2 Size)
            : base(IsleLevel.FRONT_GAMEPLAY_LAYER)
        {
            Tag(GameTags.Water);
            Collider = new Hitbox(Size.X, Size.Y-6,0,6);
            this.Position = Position;

            images = new List<Sprite<int>>();
            for (int i = 0; i < Size.X; i+=16)
            {
                Sprite<int> im = OldSkullGame.SpriteData.GetSpriteInt("water");
                images.Add(im);
                im.Play(3);
                im.X = i;
                Add(im);
            }
            
        }

        public override void Update()
        {
            base.Update();
            GameLevel.Player pl = (GameLevel.Player)Scene.CollideFirst(Collider.Bounds, GameTags.Player);

            if (pl != null)
            {
                if (!WasOnWater)
                {
                    WasOnWater = true;
                    Shake = 0;
                    Speed = 2f;
                    LastX = pl.X;
                }

                Speed *= 0.995f;
                Shake += 0.08f;
                
                int affectedTile = (int)((LastX - X) / 16);
                for (int i = 0; i < images.Count; i++)
                {
                    float x = Math.Abs(i - affectedTile)*1.2f + Shake;
                    int Height = (int)(4f + 7 * (x != 0 ? (Math.Sin(x) / x) : 1) * Speed);
                    if (Height < 0) Height = 0;
                    if (Height > 7) Height = 7;

                    images[i].Play(Height);
                }
                pl.OnWater();
            }
            else
            {
                if (WasOnWater)
                {
                    WasOnWater = false;
                    OldSkullGame.Player.Player.ExitWater();
                }
                else
                {
                    Speed *= 0.995f;
                    Shake += 0.05f;
                    int affectedTile = (int)((LastX - X) / 16);
                    for (int i = 0; i < images.Count; i++)
                    {
                        float x = Math.Abs(i - affectedTile) * 2f + Shake;
                        int Height = (int)(4f + 7 * (x != 0 ? (Math.Sin(x) / x) : 1) * Speed);
                        if (Height < 0) Height = 0;
                        if (Height > 7) Height = 7;

                        images[i].Play(Height);
                    }
                }
            }
        }
    }
}
