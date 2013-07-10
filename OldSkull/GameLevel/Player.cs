using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using OldSkull.Isle.Environment;

namespace OldSkull.GameLevel
{
    public class Player : PlatformerObject
    {
        private const int CONTEXT_MENU_TIMER = 30;

        private string imageName;
        private Isle.IsleLevel Level;
        public Isle.Drop Holding { get { return Stats.Holding; } set {Stats.Holding = value; } }
        private int side=1;

        private bool Crouching = false;
        private bool LetGo = false;
        private int useKeyTimer = 0;
        private bool UsingItem = false;
        private Isle.Container SelectedContainer;
        private int Invulnerable=0;
        public Isle.Drop SelectedDrop { get; private set; }

        private OldSkull.Isle.PlayerStats Stats { get { return OldSkullGame.Player; } }

        public Player(Vector2 position, Vector2 size,string imageName)
            : base(position, size)
        {
            this.imageName = imageName;
            AirDamping.X = 0.9f;
            GroundDamping.X = 0.9f;

            MaxSpeed = new Vector2(2f, 3);
            image = OldSkullGame.SpriteData.GetSpriteString(imageName);
            image.Play("idle", true);
            Add(image);
        }

        public override void Update()
        {
            base.Update();
            TrackPosition();
            UpdateColisions();
            UpdateControls();
            if (!onGround)
            {
                if (Speed.Y > 0) image.Play("jumpDown");
                else if (Speed.Y < 0) image.Play("jumpUp");
            }
            UpdateHud();

            Invulnerable--;
        }

        private void UpdateHud()
        {
            string action = "";
            if (SelectedContainer != null)
            {
                action = SelectedContainer.Action;
                if (Holding == null && action == "plant") action = "";
            }

            if (action == "" && Holding != null) action = Holding.Action;
            Level.Hud.action = action;
        }

        private void UpdateColisions()
        {
            SelectedContainer = (Isle.Container)Level.CollideFirst(Collider.Bounds, GameTags.Container);
            if (SelectedContainer != null)
            {
                SelectedContainer.Select();
                Level.Hud.action = SelectedContainer.Action;
            }

            SelectedDrop = (Isle.Drop)Level.CollideFirst(Collider.Bounds, GameTags.Drop);
            if (SelectedDrop != null)
            {
                SelectedDrop.Select();
            }

            Enemy Enemy = (Enemy)Level.CollideFirst(Collider.Bounds, GameTags.Enemy);
            if (Enemy != null)
            {
                TakeDamage(1);
            }
        }

        private void TakeDamage(int p)
        {
            if (Invulnerable <= 0)
            {
                Stats.Body -= 0.1f;
                Invulnerable = 50;
            }
        }

        private void TrackPosition()
        {
            if (X > Level.Width)
            {
                Level.GoToMap(Isle.IsleLevel.Side.Right);
            }
            else if (X < 0)
            {
                Level.GoToMap(Isle.IsleLevel.Side.Left);
            }
        }

        private void UpdateControls()
        {
            if (!UsingItem)
            {
                if (!Crouching)
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

                        if (!Crouching) image.Play("walk");
                    }
                    else
                    {
                        if (!Crouching && image.CurrentAnimID != "crouchOut") image.Play("idle");
                        Speed.X *= 0.9f;
                    }

                    if (KeyboardInput.pressedInput("jump"))
                    {
                        if (onGround) Speed.Y = -3.8f;
                    }
                    else if (!KeyboardInput.checkInput("jump") && (Speed.Y < 0))
                    {
                        Speed.Y *= 0.7f;
                    }


                    if (KeyboardInput.checkInput("use"))
                    {
                        useKeyTimer++;

                        if (useKeyTimer >= CONTEXT_MENU_TIMER)
                        {
                            ((Isle.IsleLevel)Level).showContext(Holding, this);
                            UsingItem = true;
                        }
                    }
                    else
                    {

                        if (useKeyTimer > 0 && useKeyTimer < CONTEXT_MENU_TIMER)
                        {
                            DefaultUse();
                        }
                        useKeyTimer = 0;
                    }

                }

                //Crouching and Pickup
                if (KeyboardInput.checkInput("down"))
                {
                    useKeyTimer = 0;
                    if (Holding != null)
                    {
                        dropItem();
                    }
                    if (!Crouching && !LetGo && onGround)
                    {
                        image.Play("crouchIn", true);
                        Crouching = true;
                    }
                }
                else
                {
                    LetGo = false;
                    if (Crouching)
                    {
                        image.Play("crouchOut", true);
                        image.OnAnimationComplete = CompleteAnimation;
                        Crouching = false;

                        if (Holding == null && SelectedDrop != null) onPickUp(SelectedDrop);
                    }
                    else if (KeyboardInput.pressedInput("up") && Holding!=null)
                    {
                        Stats.StoreItem(Holding);
                        Holding.onPlace();
                        Holding = null;
                    }
                }
            }
        }

        public void onPickUp(Isle.Drop e)
        {
            if (e != null)
            {
                e.onPickUp(this);
                Holding = e;
            }
        }

        public void DefaultUse()
        {
            if (Holding != null)
            {
                if (!PlaceItem())
                {
                    Holding.onUse(this);
                }

            }
            else if (SelectedContainer != null && SelectedContainer.CanHarvest)
            {
                SelectedContainer.Harvest();
            }
        }

        internal bool PlaceItem()
        {
            if (SelectedContainer != null && SelectedContainer.Empty)
            {
                SelectedContainer.Place(Holding);
                Holding.onPlace();
                Holding = null;
                LetGo = true;
                return true;
            }
            else
            {
                return false;
            }

        }

        public void dropItem()
        {
            Holding.onDropped();
            Holding.Speed = Speed;
            Holding.Speed.Y -= 1;
            Holding = null;
            LetGo = true;
        }

        public void stopUsing()
        {
            UsingItem = false;
        }

        public override void Added()
        {
            base.Added();
            Level = (Isle.IsleLevel)Scene;
        }

        public void CompleteAnimation(Sprite<String> sprite)
        {
            if (sprite.CurrentAnimID=="crouchOut") sprite.Play("idle", true);
        }

        public override void Render()
        {
            if (Invulnerable > 0 && Invulnerable % 10 < 5)
            {
                image.RenderFilled(OldSkullGame.Color[3]);
                //image.DrawFilledOutline(OldSkullGame.Color[3]);
            }
            else
            {
                //image.DrawFilledOutline(OldSkullGame.Color[2]);
                base.Render();
            }
        }

        public Vector2 HandPosition { get { return new Vector2(Position.X+10*side,Position.Y+3); } }
    }
}
