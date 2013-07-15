using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using OldSkull;
using OldSkull.Isle;
using OldSkull.Menu;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class TextBox:Entity
    {
        private Text[] TextLines;
        private string[] TextStrings;

        private Sprite<int> Next;

        private bool Play = false;
        private float CurrentChar = 0;
        private int CurrentLine = 0;

        private Action<TextBox> OnComplete;
        public Action<MenuButton> Choice;
        private bool Complete = false;
        private IsleLevel Level { get { return (IsleLevel)Scene; } }

        Menu.SelectorMenu MyMenu;

        public TextBox(Action<TextBox> onComplete)
            : base(IsleLevel.HUD_LAYER)
        {
            Image Bg = new Image(OldSkullGame.Atlas["ui/DialogBg"]);
            Bg.Position = new Vector2(5,Engine.Instance.Screen.Height - 35);
            Add(Bg);
            Next = OldSkullGame.SpriteData.GetSpriteInt("nextPointer");
            Next.Play(0);
            Add(Next);
            Next.Position = new Vector2(Engine.Instance.Screen.Width - 10, Engine.Instance.Screen.Height - 10);
            Next.Visible = false;

            TextLines = new Text[3];
            TextStrings = new string[3];

            OnComplete = onComplete;
        }



        public override void Update()
        {
            if (Level.CurrentState == IsleLevel.GameState.Talk)
            {
                base.Update();
                if (MyMenu != null) MyMenu.Update();
                if (KeyboardInput.pressedInput("use") || KeyboardInput.pressedInput("jump"))
                {
                    if (Complete) DefaultComplete();
                    else
                    {
                        Play = false;
                        Complete = true;
                        int i = 0;
                        foreach (string t in TextStrings)
                        {
                            if (t != null) TextLines[i].DrawText = t;
                            i++;
                        }
                    }
                }

                if (Play)
                {
                    if (CurrentLine < 3 && TextStrings[CurrentLine] != null)
                    {
                        if (CurrentChar > TextStrings[CurrentLine].Length)
                        {
                            CurrentChar = 0;
                            CurrentLine++;
                        }
                        else if ((int)CurrentChar < TextStrings[CurrentLine].Length)
                        {
                            TextLines[CurrentLine].DrawText = TextStrings[CurrentLine].Substring(0, (int)CurrentChar);
                            CurrentChar += 0.3f;
                        }
                    }
                    else
                    {
                        Complete = true;
                        Play = false;
                    }
                }
                else Next.Visible = true;
            }
        }

        private void DefaultComplete()
        {
            if (Choice != null)
            {
                Effect effect = new Effect(10, 0.85f, 1.2f, SelectorMenuEffects.ColorIn, SelectorMenuEffects.ColorOut);
                effect.outline = Color.Black;
                effect.selectedColor = OldSkullGame.Color[3];
                effect.deselectedColor = OldSkullGame.Color[2];

                MyMenu = new Menu.SelectorMenu(new string[] { "YES", "NO" }, new Action<MenuButton>[] { Choice, null }, AfterChoice, effect, false, LayerIndex);
                MyMenu.X = Engine.Instance.Screen.Width / 2;
                MyMenu.Y = Engine.Instance.Screen.Height - 33;
                MyMenu.updateButtons();

                Level.Add(MyMenu);
                Level.UpdateEntityLists();
                foreach (Text t in TextLines) t.Visible=false;
                Choice = null;
            } else OnComplete(this);
        }

        private void AfterChoice(int i)
        {
            OnComplete(this);
            MyMenu.RemoveSelf();
            MyMenu = null;
        }

        public override void Render()
        {
            if (Level.CurrentState == IsleLevel.GameState.Talk)
            {
                base.Render();
            }
        }

        public void Start(string text)
        {
            foreach (Text t in TextLines) if (t!=null) t.RemoveSelf();

            Play = true;
            Complete = false;
            CurrentChar = 0;
            CurrentLine = 0;
            Next.Visible=false;
            TextStrings = WrapText(OldSkullGame.Font,text,140);

            int i = 0;
            foreach (string t in TextStrings)
            {
                if (t != null)
                {
                    TextLines[i] = new Text(OldSkullGame.Font, "", new Vector2(11, Engine.Instance.Screen.Height - 27 + 8*i), Text.HorizontalAlign.Left);
                    Add(TextLines[i]);
                }
                i++;
            }
        }

        public string[] WrapText(Microsoft.Xna.Framework.Graphics.SpriteFont spriteFont, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');

            string[] sb = new string[3];
            int currentLine = 0;

            float lineWidth = 0f;

            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb[currentLine] += word + " ";
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    currentLine++;
                    sb[currentLine] += word + " ";
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb;
        }
    }
}
