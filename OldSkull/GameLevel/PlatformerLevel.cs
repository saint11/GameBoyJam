using System;
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
        public enum GameMode { Quest };

        //Layers
        private Layer bgGameLayer;
        private Layer gameLayer;
        private Layer hudLayer;
        private Layer pauseLayer;

        //Layer Constants
        public static readonly int BG_GAME_LAYER = -3;
        public static readonly int GAMEPLAY_LAYER = 0;
        public static readonly int HUD_LAYER = 3;
        public static readonly int PAUSE_LAYER = 4;
        public static readonly int REPLAY_LAYER = 10;

        //LevelLoader Variables
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Gravity = new Vector2(0f,0.1f);

        //Lists
        public List<Entity> Solids {get;private set;}

        //Camera
        public Entity CameraTarget;

        public PlatformerLevel(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            
            SetLayer(BG_GAME_LAYER, bgGameLayer = new Layer());
            SetLayer(GAMEPLAY_LAYER, gameLayer = new Layer());
            SetLayer(HUD_LAYER, hudLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));
            SetLayer(PAUSE_LAYER, pauseLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));

            Solids = new List<Entity>();
        }

        internal void loadLevel(PlatformerLevelLoader ll)
        {
            foreach (Solid solid in ll.solids)
            {
                Add(solid);
                Solids.Add(solid);
            }
            foreach (XmlElement e in ll.entities)
            {
                LoadEntity(e);
            }

            Add(new SolidGrid(ll.solidGrid));
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
            if (Camera.Y + Camera.Viewport.Height > Height) Camera.Y = Height - Camera.Viewport.Width;
        }
    }
}
