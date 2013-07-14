#region Using Statements
using Monocle;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using OldSkull.Isle;
#endregion

namespace OldSkull
{
    
    public class OldSkullGame : Engine
    {
        static public Atlas Atlas;
        static public SpriteData SpriteData;
        static public SpriteFont Font;
        static public PlayerStats Player;

        static public Color[] Color;

        public int PlayTime=0;

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
            Atlas = new Atlas( Path + @"Content/Atlas/atlas.xml", true);
            SpriteData = new SpriteData(Path + @"Content/Atlas/SpriteData.xml", Atlas);
            Content.RootDirectory = Path+"Content";
            Font = Content.Load<SpriteFont>(@"Misc/pixel");

            Color = new Color[] { new Color(76,42,4), new Color(148,122,76), new Color(196,174,148), new Color(252,235,229) };
        }

        protected override void Initialize()
        {
            base.Initialize();
            Screen.Scale = 2f;

            KeyboardInput.InitDefaultInput();
            KeyboardInput.Add("jump", Keys.Z);
            KeyboardInput.Add("use", Keys.X);
            KeyboardInput.Add("pause", Keys.Space);
            Scene = new Isle.MainMenu();
        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            PlayTime++;
            if (PlayTime == int.MaxValue) PlayTime = 0;
        }
        public static int GetTotalTime() { return ((OldSkullGame)Instance).PlayTime; }
    }
}
