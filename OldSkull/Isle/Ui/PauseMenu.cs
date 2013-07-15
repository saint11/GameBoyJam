using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monocle;
using Microsoft.Xna.Framework;
using OldSkull.Menu;


namespace OldSkull.Isle.Ui
{
    public class PauseMenu : Entity
    {
        private IsleLevel Level { get { return (IsleLevel)Scene; } }
        private SelectorMenu Menu;
        private Action ExitFunction;
        private Image Back;

        private Text Holding;
        private Text Lives;
        private Text Coins;

        public PauseMenu()
            : base(IsleLevel.PAUSE_LAYER)
        {
            Back = new Image(OldSkullGame.Atlas["ui/pauseBase"]);
            X = Engine.Instance.Screen.Width;
            Add(Back);
        }

        private void UpdateMenu()
        {
            Effect effect = new Effect(10, 0.85f, 1.2f, SelectorMenuEffects.ColorIn, SelectorMenuEffects.ColorOut);
            effect.outline = Color.Black;
            effect.selectedColor = OldSkullGame.Color[2];
            effect.deselectedColor = OldSkullGame.Color[0];

            string[] itemList = new string[OldSkullGame.Player.Inventory.Count];
            Action[] actionList = new Action[OldSkullGame.Player.Inventory.Count];
            for (int i = 0; i < OldSkullGame.Player.Inventory.Count; i++)
            {
                itemList[i] = OldSkullGame.Player.Inventory[i].Name;
                actionList[i] = OldSkullGame.Player.Inventory[i].onSwitch;
            }

            Menu = new SelectorMenu(itemList, actionList, SwitchItems, effect, false, IsleLevel.PAUSE_LAYER);
            Menu.X = Engine.Instance.Screen.Width / 2 + X;
            Menu.Y = 30;

            if (OldSkullGame.Player.Holding!=null)
                Holding = new Text(OldSkullGame.Font, OldSkullGame.Player.Holding.Name.ToUpper(),
                    new Vector2(Engine.Instance.Screen.Width / 2, 20));
            else
                Holding = new Text(OldSkullGame.Font, "NOTHING",
                    new Vector2(Engine.Instance.Screen.Width / 2, 20));
            Holding.Color = OldSkullGame.Color[0];


            Lives = new Text(OldSkullGame.Font, "X" + OldSkullGame.Player.Lives.ToString(), new Vector2(145, 35));
            Add(Lives);
            Coins = new Text(OldSkullGame.Font, "X" + OldSkullGame.Player.Coin.ToString(), new Vector2(145, 68));
            Add(Coins);

            if (Level != null)
            {
                Menu.Active = false;
                Menu.Visible = false;
                Level.Add(Menu);
                if (Holding!=null) Add(Holding);
            }
        }

        private void SwitchItems(int index)
        {
            GameLevel.Player Player = OldSkullGame.Player.Player;
            Isle.Drop PickUp = OldSkullGame.Player.Inventory[index];
            Isle.Drop Drop = Player.Holding;


            //OldSkullGame.Player.Holding.onPlace();
            if (Drop != null)
            {
                Drop.onPlace();
                OldSkullGame.Player.Inventory[index] = Drop;
            }
            else
            {
                OldSkullGame.Player.Inventory.RemoveAt(index);
            }
            Player.onPickUp(PickUp);
            Level.Add(PickUp);
            


            Level.UpdateEntityLists();
            Menu.RemoveSelf();
            if (Holding != null) Holding.RemoveSelf();
            Menu.Active = false;
            Menu.Visible = false;

            UpdateMenu();
            Menu.Active = true;
            Menu.Visible = true;
            Level.UpdateEntityLists();

            Holding.Scale = new Vector2(2f);
            Tween.Scale(Holding, new Vector2(1), 10, Ease.BackInOut, Tween.TweenMode.Oneshot);

            Menu.selected = index;
            Menu.updateButtons();
        }

        public void Call()
        {
            Tween.Position(this, Vector2.Zero, 10, Ease.BackOut, Tween.TweenMode.Oneshot);
            UpdateMenu();
            Menu.Visible = true;
            Menu.Active = true;
            Level.UpdateEntityLists();
        }

        public void onComplete(Tween tween)
        {
            ExitFunction();
        }

        public void Retract(Action ExitFunction)
        {
            this.ExitFunction = ExitFunction;
            Level.UpdateEntityLists();
            Menu.RemoveSelf();
            if (Holding != null) Holding.RemoveSelf();
            Menu.Active = false;
            Menu.Visible = false;

            Tween.Position(this, new Vector2(Engine.Instance.Screen.Width, 0), 10, Ease.BackOut, Tween.TweenMode.Oneshot).OnComplete = onComplete;
        }


        public override void Update()
        {
            if (Level.CurrentState == IsleLevel.GameState.Paused)
            {
                base.Update();
                if (Menu!=null)Menu.Update();
            }
        }

        public override void Render()
        {
            if (Level.CurrentState == IsleLevel.GameState.Paused)
            {
                base.Render();
                if (Menu != null)
                {
                    Menu.X = Engine.Instance.Screen.Width / 2 + X;
                    Menu.Render();
                }
            }
        }
    }
}
