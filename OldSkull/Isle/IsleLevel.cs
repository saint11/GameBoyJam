using System;
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

        private Side from;
        private float lastPlayerPosition = -1;
        public Player player;
        private int MapNumber;

        private PauseMenu PauseMenu;
        public bool Paused = false;
        private Entity Transit;

        public IsleLevel(PlatformerLevelLoader loader, Side from, int MapNumber)
            : base((int)loader.size.X, (int)loader.size.Y)
        {
            this.from = from;
            loadLevel(loader);
            this.MapNumber = MapNumber;
            PauseMenu = new PauseMenu();
            Add(PauseMenu);

            Transition.TransitionOut(this,PAUSE_LAYER);
        }


        public override void Begin()
        {
            base.Begin();
            Add(new TilableBackground("sky", SKY_GAME_LAYER));

            Hud = new Hud();
            Add(Hud);

            skyGameLayer.CameraMultiplier = 0.8f;
            Camera.Position = CameraTarget.Position-Engine.Instance.Screen.Size/2;
        }

        public override void LoadEntity(XmlElement e)
        {

            if (e.Name == "Player")
            {
                if (lastPlayerPosition == -1)
                {
                    player = new Player(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(10, 24), "jonathan");
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
                Add(new Enemy(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), new Vector2(10,24) , "skull"));
//                Add(new Drop(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.Attr("Type")));
            }
            else if (e.Name == "Fruit" || e.Name == "Throwable")
            {
                Add(new Drop(new Vector2(e.AttrFloat("x"), e.AttrFloat("y")), e.Attr("Type")));
            }
            else if (e.Name == "SoftGround")
            {
                Add(new Container(new Vector2(e.AttrFloat("x"), e.AttrFloat("y"))));
            }
        }

        internal void showContext(Drop Holding, Player player)
        {
            Add(new Isle.ContextMenu(Holding,player));
        }


        internal void GoToMap(Side side)
        {
            Paused = true;
            Transit = Transition.TransitionIn(this, PAUSE_LAYER, () =>
            {
                Engine.Instance.Scene = new Isle.Map.WorldMap(MapNumber, side);
            });
        }

        internal void Pause()
        {
            Paused = true;
            PauseMenu.Call();   
        }



        internal void UnPause()
        {
            PauseMenu.Retract(() => { Paused = false; KeyboardInput.Active = true; });
        }

        public override void Update()
        {
            if (!Paused)
            {
                base.Update();
                OldSkullGame.Player.Update();
                if (KeyboardInput.pressedInput("pause")) Pause();
            }
            else
            {
                KeyboardInput.Update();
                PauseMenu.Update();
                if (Transit != null)
                {
                    if (Transit.Scene == null) UpdateEntityLists();
                    Transit.Update();
                }
                if (KeyboardInput.pressedInput("pause"))
                {
                    UnPause();
                    KeyboardInput.Active = false;
                }
            }

            if (OldSkullGame.Player.Body <= 0) Engine.Instance.Scene = new GameOver();
        }
    }
}

