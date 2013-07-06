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
        private int width;
        private int height;
        public Vector2 Gravity = new Vector2(0f,0.1f);

        //Lists
        public List<Entity> Solids {get;private set;}

        //Camera
        public Entity CameraTarget;

        public PlatformerLevel(int width, int height)
        {
            this.width = width;
            this.height = height;
            
            SetLayer(BG_GAME_LAYER, bgGameLayer = new Layer());
            SetLayer(GAMEPLAY_LAYER, gameLayer = new Layer());
            SetLayer(HUD_LAYER, hudLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));
            SetLayer(PAUSE_LAYER, pauseLayer = new Layer(BlendState.AlphaBlend, SamplerState.PointClamp, 0));

            Solids = new List<Entity>();
        }

        public override void Begin()
        {
            base.Begin();
        }

        internal void loadLevel(PlatformerLevelLoader ll)
        {
            foreach (Solid solid in ll.solids)
            {
                Add(solid);
                Solids.Add(solid);
            }
        }

        public override void Update()
        {
            base.Update();

            if (CameraTarget != null)
            {
                Camera.X = Calc.LerpSnap(Camera.X,CameraTarget.X - Camera.Viewport.Width / 2,0.1f);
                Camera.Y = Calc.LerpSnap(Camera.Y, CameraTarget.Y - Camera.Viewport.Height / 2, 0.1f);
            }
            
            KeyboardInput.Update();
        }
    }
}
