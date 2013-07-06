#region Using Statements
using Monocle;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Collections.Generic;
using System.Collections;
#endregion

namespace OldSkull
{
    
    public class OldSkullGame : Engine
    {
        static public Atlas Atlas;
        static public SpriteData SpriteData;

        public const string Path = @"Assets\";

        static void Main(string[] args)
        {
            using (OldSkullGame demo = new OldSkullGame())
            {
                demo.Run();
            }
        }

        public OldSkullGame()
            : base(160, 144, 60f, "Isle of the Dead")
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Atlas = new Atlas("Assets/Content/Atlas/atlas.xml", true);
            SpriteData = new SpriteData(@"Content\Atlas\SpriteData.xml", Atlas);
        }

        protected override void Initialize()
        {
            base.Initialize();
            Screen.Scale = 2f;

            KeyboardInput.InitDefaultInput();
            KeyboardInput.Add("jump", Keys.Z);
            Scene = new Isle.MainMenu();
        }
    }
}
