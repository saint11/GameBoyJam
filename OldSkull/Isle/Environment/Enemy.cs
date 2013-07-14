using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull.GameLevel;
using OldSkull;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldSkull.Isle.Environment
{
    public class Enemy : PlatformerObject
    {
        private enum CurrentMove { Walk, Stand, Chase };
        private CurrentMove move;
        private int timer;
        private int Side = 1;
        private int Invulnerable = 0;
        private float Hp = 1;

        public Enemy(Vector2 Position, Vector2 Size, string Type)
            :base(Position,Size)
        {
            image = OldSkullGame.SpriteData.GetSpriteString(Type);
            image.Play("idle");
            Add(image);
            timer = 30;
            Tag(GameTags.Enemy);
        }

        public override void Update()
        {
            base.Update();

            if (Vector2.Distance(Position, Level.player.Position) < 60) move = CurrentMove.Chase;

            if (move == CurrentMove.Walk)
            {
                //avoid falling
                Rectangle check = Collider.Bounds;
                check.X += (int)Collider.Width*Side;
                check.Y += (int)Collider.Height;
                if (onGround && !Scene.CollideCheck(check, GameTags.Solid) && Calc.Random.Chance(0.5f)) Side *= -1;
                    
                MoveH(0.3f);
                AdvanceTime();
            }
            else if (move == CurrentMove.Stand)
            {
                Speed.X = 0;
                image.Play("idle");
                AdvanceTime();
            } else if (move == CurrentMove.Chase)
            {
                Side = Math.Sign(Level.player.X - X);
                MoveH(0.6f);
                if (Vector2.Distance(Position, Level.player.Position) < 60) move = CurrentMove.Walk;
                Level.player.AddSoul(-0.0005f);
            }

            image.Effects = Speed.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Invulnerable--;
        }

        private void MoveH(float HSpeed)
        {
            Rectangle check = Collider.Bounds;
            check.X += Side*2;
            if (onGround && Scene.CollideCheck(check, GameTags.Solid))
            {
                check.Y -= 16;
                if (!Scene.CollideCheck(check, GameTags.Solid))
                {
                    Speed.Y = -2.2f;
                }
                else
                {
                    check.Y -= 16;
                    if (!Scene.CollideCheck(check, GameTags.Solid))
                        Speed.Y = -2.8f;
                }
            }
            Speed.X = HSpeed * Side;
            if (onGround) image.Play("walk");
            else
            {
                if (Speed.Y > 0) image.Play("jumpUp");
                else image.Play("jumpDown");
            }
        }

        private void AdvanceTime()
        {
            timer--;
            if (timer == 0)
            {
                move = Calc.Choose<CurrentMove>(Calc.Random, new CurrentMove[] { CurrentMove.Walk, CurrentMove.Stand });
                timer = Calc.Random.Range(30, 120);
                Side = Calc.Random.NextFloat() > 0.5 ? 1 : -1;
            }
        }

        private IsleLevel Level { get { return (IsleLevel)Scene; } }

        public void TakeDamage(float damage, Vector2 source)
        {
            if (Invulnerable <= 0)
            {
                Hp -= damage;
                Invulnerable = 50;

                Speed.Y = -1f;
                Speed.X -= 3 * Math.Sign(source.X - Position.X);
            }

            if (Hp <= 0)
            {
                onDeath();
                Level.player.AddSoul(0.1f);
            }
        }

        private void onDeath()
        {
            Level.Add(new Fx.Explosion(Position, LayerIndex));
            RemoveSelf();
        }

        public override void  Render()
        {
            if (Invulnerable > 0 && Invulnerable % 10 < 5)
            {
                image.RenderFilled(OldSkullGame.Color[2]);
            }
            else
            {
                base.Render();
            }
        }
        
    }
}
