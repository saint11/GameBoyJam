﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OldSkull.GameLevel
{
    public class PlatformerLevel : Scene
    {
        //Layers
        protected Layer skyGameLayer;
        protected Layer bgGameLayer;
        protected Layer gameLayer;
        protected Layer hudLayer;
        protected Layer pauseLayer;

        //Layer Constants
        public static readonly int SKY_GAME_LAYER = -5;
        public static readonly int BG_GAME_LAYER = -3;
        public static readonly int GAMEPLAY_LAYER = 0;
        public static readonly int FRONT_GAMEPLAY_LAYER = 1;
        public static readonly int HUD_LAYER = 3;
        public static readonly int PAUSE_LAYER = 4;
        public static readonly int REPLAY_LAYER = 10;

        //LevelLoader Variables
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Gravity = new Vector2(0f,0.1f);
        public string Name { get; private set; }

        //Lists
        public List<Entity> Solids {get;private set;}

        //Camera
        public Entity CameraTarget;
        protected string ConnectionRight;
        protected string ConnectionLeft;

        public PlatformerLevel(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            SetLayer(SKY_GAME_LAYER, skyGameLayer = new Layer());
            SetLayer(BG_GAME_LAYER, bgGameLayer = new Layer());
            SetLayer(GAMEPLAY_LAYER, gameLayer = new Layer());
            SetLayer(FRONT_GAMEPLAY_LAYER, gameLayer = new Layer());
            SetLayer(HUD_LAYER, hudLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));
            SetLayer(PAUSE_LAYER, pauseLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));

            Solids = new List<Entity>();
        }

        internal virtual void loadLevel(PlatformerLevelLoader ll)
        {
            Name = ll.Name;
            foreach (Solid solid in ll.solids)
            {
                Add(solid);
                Solids.Add(solid);
            }
            foreach (XmlElement e in ll.entities)
            {
                LoadEntity(e);
            }

            foreach (XmlElement e in ll.tilesets)
            {
                LoadTileset(e);
            }

            ConnectionRight = ll.Right;
            ConnectionLeft = ll.Left;
            Add(new SolidGrid(ll.solidGrid));
        }

        public virtual void LoadTileset(XmlElement e)
        {
            Add(new Graphics.Tileset(-3, e.InnerText, OldSkullGame.Atlas["tilesets/"+e.Attr("tileset")]));
        }

        public virtual void LoadEntity(XmlElement e)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            base.Update();

            if (CameraTarget != null)
            {
                Camera.X = Calc.LerpSnap(Camera.X,CameraTarget.X - Camera.Viewport.Width / 2,0.1f);
                Camera.Y = Calc.LerpSnap(Camera.Y, CameraTarget.Y - Camera.Viewport.Height / 2, 0.1f);
                
            }

            KeepCameraOnBounds();
            
            KeyboardInput.Update();
        }

        private void KeepCameraOnBounds()
        {
            if (Camera.X < 0) Camera.X = 0;
            if (Camera.X + Camera.Viewport.Width > Width) Camera.X = Width - Camera.Viewport.Width;
            if (Camera.Y < 0) Camera.Y = 0;
            if (Camera.Y + Camera.Viewport.Height > Height) Camera.Y = Height - Camera.Viewport.Height;
        }
        
        
    }
}
