using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Monocle;
using Microsoft.Xna.Framework;
using OldSkull.Menu;
using OldSkull.GameLevel;

namespace OldSkull.Isle.Map
{
    class Node : Entity
    {
        private Image image;
        private bool Selected=false;
        private string UpConnect;
        private string DownConnect;
        private string LeftConnect;
        private string RightConnect;
        private string Id;

        public string Name { get; private set; }

        public Node(Vector2 Position, string name, string id, string up, string down, string left, string right)
            : base(0)
        {
            this.Name = name;
            this.Position = Position;
            this.UpConnect = up;
            this.DownConnect = down;
            this.LeftConnect = left;
            this.RightConnect = right;
            this.Id = id;

            image = new Image(OldSkullGame.Atlas["map/point"]);
            image.CenterOrigin();
            Add(image);
        }

        public override void Render()
        {
            if (Selected) image.DrawFilledOutline(OldSkullGame.Color[3]);
            base.Render();
        }

        public override void Update()
        {
            base.Update();
            if (Selected)
            {
                WorldMap Map = (WorldMap) Scene;
                if (KeyboardInput.pressedInput("up"))
                {
                    if (Map.Select(UpConnect)) Map.lastPressed = IsleLevel.Side.Left;
                }
                else if (KeyboardInput.pressedInput("down"))
                {
                    if (Map.Select(DownConnect)) Map.lastPressed = IsleLevel.Side.Right;
                }
                else if (KeyboardInput.pressedInput("left"))
                {
                    if (Map.Select(LeftConnect)) Map.lastPressed = IsleLevel.Side.Right;
                }
                else if (KeyboardInput.pressedInput("right"))
                {
                    if (Map.Select(RightConnect)) Map.lastPressed = IsleLevel.Side.Left;
                }
                else if (KeyboardInput.pressedInput("jump") || KeyboardInput.pressedInput("use"))
                {
                    PlatformerLevelLoader loader = PlatformerLevelLoader.load(Id);
                    PlatformerLevel level = new IsleLevel(loader,Map.lastPressed,Map.Selected);
                    OldSkullGame.Instance.Scene = level;
                }

            }
        }

        internal void Deselect()
        {
            Selected = false;
        }

        internal void Select()
        {
            Selected = true;
        }
    }

}