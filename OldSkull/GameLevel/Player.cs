﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.GameLevel
{
    class Player : PlatformerObject
    {
        private const int CONTEXT_MENU_TIMER = 30;

        private string imageName;
        private Isle.IsleLevel Level;
        public Isle.Drop Holding;
        private int side=1;

        private bool Crouching = false;
        private bool LetGo = false;
        private int useKeyTimer = 0;
        private bool UsingItem = false;
        private Isle.Container SelectedContainer;
        

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
            TrackPosition();
            UpdateColisions();
            UpdateControls();
            UpdateHud();
        }

        private void UpdateHud()
        {
            string action = "";
            if (SelectedContainer != null && Holding!=null) action = SelectedContainer.Action;
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
        }

        private void TrackPosition()
        {
            if (X > Level.Width)
            {
                Engine.Instance.Scene = new Isle.WorldMap(0);
            }
            else if (X < 0)
            {
                Engine.Instance.Scene = new Isle.WorldMap(0);
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
                        if (onGround) Speed.Y = -4;
                    }
                    else if (!KeyboardInput.checkInput("jump") && (Speed.Y < 0))
                    {
                        Speed.Y *= 0.7f;
                    }


                    if (KeyboardInput.checkInput("use"))
                    {
                        if (Holding != null)
                        {
                            useKeyTimer++;

                            if (useKeyTimer >= CONTEXT_MENU_TIMER)
                            {
                                ((Isle.IsleLevel)Level).showContext(Holding, this);
                                UsingItem = true;
                            }
                        }
                    }
                    else
                    {

                        if (Holding != null && useKeyTimer > 0 && useKeyTimer < CONTEXT_MENU_TIMER)
                        {
                            defaultUseHolding();
                        }
                        useKeyTimer = 0;
                    }

                }

                //Crouching and Pickup
                if (KeyboardInput.checkInput("down"))
                {
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

                        if (Holding == null)
                        {
                            Isle.Drop e = (Isle.Drop)Level.CollideFirst(Collider.Bounds, GameTags.Drop);
                            if (e != null)
                            {
                                PickUp(e);
                            }
                        }
                    }
                }
            }
        }

        private void PickUp(Isle.Drop e)
        {
            e.onPickUp(this);
            Holding = e;
        }

        public void defaultUseHolding()
        {
            if (Holding != null)
            {
                if (SelectedContainer != null && SelectedContainer.Empty)
                {
                    SelectedContainer.Place(Holding);
                    Holding.onPlace();
                    Holding = null;
                    LetGo = true;
                }
                else
                {
                    Holding.onUse(this);
                }

            }
        }

        public void dropItem()
        {
            Holding.onDropped();
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

            MaxSpeed = new Vector2(2f,3);
            image = OldSkullGame.SpriteData.GetSpriteString(imageName);
            Add(image);
            image.Play("idle", true);
            Level = (Isle.IsleLevel)Scene;
        }

        public void CompleteAnimation(Sprite<String> sprite)
        {
            if (sprite.CurrentAnimID=="crouchOut") sprite.Play("idle", true);
        }

        public override void Render()
        {
            //image.DrawOutline(OldSkullGame.Color[3]);
            base.Render();
        }

        public Vector2 HandPosition { get { return new Vector2(Position.X+8*side,Position.Y-1); } }
    }
}
