﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using OldSkull.Graphics;
using Monocle;
using Microsoft.Xna.Framework;
using System.Xml;
using OldSkull.Isle.Ui;
using OldSkull.Isle.Environment;


namespace OldSkull.Isle
{
    public class IsleLevel : PlatformerLevel
    {
        public Hud Hud;
        public enum Side { Left, Right, Secret };
        public enum GameState { Game, Paused, Talk, Transition, ExitGame };

        public GameState CurrentState = GameState.Game;

        private Side from;
        private float lastPlayerPosition = -1;
        public Player player;
        private int MapNumber;

        public TextBox TalkBox;
        private PauseMenu PauseMenu;
        private Entity Transit;
        private bool MarkedForGameOver=false;

        private Menu.SelectorMenu ExitGameMenu;
        private Entity ExitMenuBg;

        public IsleLevel(PlatformerLevelLoader loader, Side from, int MapNumber)
            : base((int)loader.size.X, (int)loader.size.Y)
        {
            this.from = from;
            loadLevel(loader);
            
            this.MapNumber = MapNumber;
            PauseMenu = new PauseMenu();
            Add(PauseMenu);

            TalkBox = new TextBox(TextComplete);
            Add(TalkBox);
            Transition.TransitionOut(this,PAUSE_LAYER);

            
        }

        private Vector2 GetStartingPosition(Side from,Rectangle Collider)
        {
            Vector2 Position = Vector2.Zero;

            Position.X = from == Side.Left ? Width - 16 : 16;

            bool UntilTrue = true;

            Collider.X = (int)Position.X;
            Collider.Y = (int)Position.Y;
            if (CollideCheck(Collider,GameTags.Solid)) UntilTrue = false;

            for (int i = (int)Position.Y; i < Height; i+=16)
            {
                Collider.X = (int)Position.X;
                Collider.Y = (int)Position.Y;

                if (CollideCheck(Collider, GameTags.Solid) == UntilTrue)
                {
                    if (UntilTrue) break;
                    else UntilTrue = true;
                }
                else
                {
                    Position.Y = i;
                }
            }
            
            Position.Y += 4;
            return Position;
        }


        public override void Begin()
        {
            base.Begin();
            Add(new TilableBackground("sky", SKY_GAME_LAYER));

            Hud = new Hud();
            Add(Hud);

            skyGameLayer.CameraMultiplier = 0.8f;

            if (player == null)
            {
                player = new Player(Vector2.Zero);
                player.Position = GetStartingPosition(from, player.Collider.Bounds);

                OldSkullGame.Player.InitPlayer(player);
                Add(player);
                CameraTarget = player;
                lastPlayerPosition = player.X;
            }

            OldSkullGame.Player.onEnterLevel(this);
            Camera.Position = CameraTarget.Position-Engine.Instance.Screen.Size/2;
        }

        public override void LoadEntity(XmlElement e)
        {

            if (e.Name == "Player" && from==Side.Secret)
            {
                if (lastPlayerPosition == -1)
                {
                    player = new Player(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")));
                    OldSkullGame.Player.InitPlayer(player);
                    Add(player);
                    CameraTarget = player;
                    lastPlayerPosition = player.X;
                }
                else
                {
                    if (player != null)
                    {
                        if (from == Side.Left && lastPlayerPosition > e.AttrFloat("x"))
                            player.Position = new Vector2(e.AttrFloat("x"), e.AttrFloat("y"));
                        if (from == Side.Right && lastPlayerPosition < e.AttrFloat("x"))
                            player.Position = new Vector2(e.AttrFloat("x"), e.AttrFloat("y"));
                    }
                }
            }
            else if (e.Name == "Skull")
            {
                Add(new Skull(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(10,24) , "skull"));
            }
            else if (e.Name == "EyeBat")
            {
                Add(new EyeBat(e));
            }
            else if (e.Name == "Npc")
            {
                Add(new Npc(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Hitbox(32, 32), e.Attr("Type"), e.Attr("Talk"), e.Attr("Wants"), e.Attr("CompleteTalk"), e.Attr("Reward")));
            }
            else if (e.Name == "Charriot")
            {
                Add(new Npc(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Hitbox(32, 32), "charriot", "%Price" + e.Attr("Price"),"","",""));
            }
            else if (e.Name == "Fruit" || e.Name == "Throwable")
            {
                UserData.ItemStats ExistingItem = UserData.GetItemStats(Name + e.Attr("id"));
                if (!ExistingItem.Valid)
                {
                    Add(new Drop(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.Attr("Type"), Name + e.Attr("id")));
                }
            }
            else if (e.Name == "SoftGround")
            {
                Container Container = UserData.GetGroundHp(Name + e.Attr("id"));
                if (Container != null)
                    Add(Container);
                else
                    Add(new Container(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), 3, Name + e.Attr("id")));
            }
            else if (e.Name == "Water")
            {
                    Add(new Fx.Water(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(e.AttrFloat("width"),16)));
            }
            else if (e.Name == "DeadlyWater")
            {
                   Add(new Environment.DeadlyWater(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(e.AttrFloat("width"), e.AttrFloat("height"))));
            }
            else if (e.Name == "Door")
            {
                if (!UserData.GetDoorOpen(Name + e.Attr("id")))
                    Add(new Door(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), Name + e.Attr("id")));
            }
        }

        internal void showContext(Drop Holding, Player player)
        {
            Add(new Isle.ContextMenu(Holding,player));
        }


        internal void OutOfBounds(Side side)
        {
            CurrentState = GameState.Transition;
            Transit = Transition.TransitionIn(this, PAUSE_LAYER, () =>
            {
                string NextlevelName = side == Side.Left ? ConnectionLeft : ConnectionRight;
                PlatformerLevelLoader loader = PlatformerLevelLoader.load(NextlevelName);
                PlatformerLevel level = new IsleLevel(loader, side, MapNumber);
                OldSkullGame.Instance.Scene = level;
                //Engine.Instance.Scene = new Isle.Map.WorldMap(MapNumber, side);
            });
            End();
        }

        internal void Pause()
        {
            CurrentState = GameState.Paused;
            PauseMenu.Call();   
        }

        internal void UnPauseExit(int choice)
        {
            ExitGameMenu.RemoveSelf();
            ExitMenuBg.RemoveSelf();
            CurrentState = GameState.Game;
        }

        internal void UnPause()
        {
            PauseMenu.Retract(() => { CurrentState = GameState.Game; KeyboardInput.Active = true; });
        }

        public void QuitGame(Menu.MenuButton Mb)
        {
            Engine.Instance.Scene = new MainMenu();
        }


        public override void Update()
        {

            if (CurrentState == GameState.Game)
            {
                base.Update();
                OldSkullGame.Player.Update();
                if (KeyboardInput.pressedInput("pause")) Pause();
                if (OldSkullGame.Player.Body <= 0 || OldSkullGame.Player.Soul <= 0) onGameOver() ;

                if (KeyboardInput.pressedInput("escape"))
                {
                    AddExitMenu();
                }
            }
            else if (CurrentState == GameState.Paused)
            {
                KeyboardInput.Update();
                PauseMenu.Update();
                
                if (KeyboardInput.pressedInput("pause"))
                {
                    UnPause();
                    KeyboardInput.Active = false;
                }
            }
            else if (CurrentState == GameState.Transition)
            {
                if (Transit != null)
                {
                    if (Transit.Scene == null) UpdateEntityLists();
                    Transit.Update();
                }
            }
            else if (CurrentState == GameState.Talk)
            {
                TalkBox.Update();
                KeyboardInput.Update();
                UpdateEntityLists();
            }
            else if (CurrentState == GameState.ExitGame)
            {
                KeyboardInput.Update();
                ExitGameMenu.Update();
                UpdateEntityLists();
            }

            if (MarkedForGameOver)
            {
                End();
                UpdateEntityLists();
                Engine.Instance.Scene = new GameOver();
            }
        }

        private void AddExitMenu()
        {
            CurrentState = GameState.ExitGame;

            Image Bg = new Image(OldSkullGame.Atlas["ui/exitGame"]);
            ExitMenuBg = new Entity(PAUSE_LAYER);
            ExitMenuBg.Add(Bg);
            Add(ExitMenuBg);
            Bg.CenterOrigin();
            Bg.X = Engine.Instance.Screen.Width / 2;
            Bg.Y = Engine.Instance.Screen.Height / 2;

            Menu.Effect effect = new Menu.Effect(10, 0.85f, 1.2f, Menu.SelectorMenuEffects.ColorIn, Menu.SelectorMenuEffects.ColorOut);
            effect.outline = Color.Black;
            effect.selectedColor = OldSkullGame.Color[3];
            effect.deselectedColor = OldSkullGame.Color[1];

            ExitGameMenu = new Menu.SelectorMenu(new string[] { "YES", "NO" }, new Action<Menu.MenuButton>[] { QuitGame, null }, UnPauseExit, effect, false, PAUSE_LAYER);
            ExitGameMenu.X = (int)Engine.Instance.Screen.Width / 2;
            ExitGameMenu.Y = (int)Engine.Instance.Screen.Height / 2-7;

            Add(ExitGameMenu);
        }
        public void TextComplete(TextBox textBox)
        {
            CurrentState = GameState.Game;
        }

        public void GoToMap ()
        {
            CurrentState = GameState.Transition;
            Transit = Transition.TransitionIn(this, PAUSE_LAYER, () =>
            {
                Engine.Instance.Scene = new Isle.Map.WorldMap(MapNumber, Side.Secret);
            });
            End();
        }

        internal override void loadLevel(PlatformerLevelLoader ll)
        {
            base.loadLevel(ll);
            foreach (Drop Item in UserData.GetItemsHere(Name))
            {
                if (Item != null) Add(Item);
            }
        }

        internal void onGameOver()
        {
            MarkedForGameOver = true;
        }
    }
}

