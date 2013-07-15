using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using OldSkull.Isle;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class PlayerStats
    {
        public Player Player {get;private set;}
        public float Body = 1;
        public float Soul = 1;

        public int Lives = 3;
        public int Coin = 0;

        private List<PlayerStatEffect> BodyEffects;
        private List<PlayerStatEffect> SoulEffects;
        public List<Drop> Inventory;
        public Isle.Drop Holding;

        public enum Attribute { Body, Soul };

        public PlayerStats(Player Player)
        {
            this.Player = Player;
            BodyEffects = new List<PlayerStatEffect>();
            SoulEffects = new List<PlayerStatEffect>();
            Inventory = new List<Drop>();
        }

        internal void InitPlayer(GameLevel.Player Player)
        {
            this.Player = Player;
        }

        public void onGameOver()
        {
            Lives--;
            if (Lives > 0)
            {
                for (int i = Inventory.Count - 1; i >= 0; i--)
                {
                    if (!Inventory[i].KeyItem)
                    {
                        Inventory.RemoveAt(i);
                    }
                }
                Body = 1;
                Soul = 1;
                BodyEffects = new List<PlayerStatEffect>();
                SoulEffects = new List<PlayerStatEffect>();
                if (Holding != null && !Holding.KeyItem) Holding = null;
            }
            else
            {
                Lives = 3;
                Coin = 0;
                Body = 1;
                Soul = 1;
                Inventory = new List<Drop>();
                BodyEffects = new List<PlayerStatEffect>();
                SoulEffects = new List<PlayerStatEffect>();
                if (Holding != null && !Holding.KeyItem) Holding = null;
            }
        }

        public void Update()
        {
            Body -= 0.00005f;

            for (int i = BodyEffects.Count; i > 0; i--)
            {
                Body += BodyEffects[i-1].Increment/1000;

                BodyEffects[i - 1].AdvanceTime();
                if (BodyEffects[i - 1].Exausted)
                {
                    BodyEffects.RemoveAt(i - 1);
                }
            }

            if (Soul > 1) Soul = 1;

            for (int i = SoulEffects.Count; i > 0; i--)
            {
                Soul += SoulEffects[i - 1].Increment / 1000;

                SoulEffects[i - 1].AdvanceTime();
                if (SoulEffects[i - 1].Exausted)
                {
                    SoulEffects.RemoveAt(i - 1);
                }
            }

            if (Body > 1) Body = 1;
        }

        internal void AddBodyEffect(PlayerStatEffect Effect)
        {
            BodyEffects.Add(Effect);
        }
        internal void AddSoulEffect(PlayerStatEffect Effect)
        {
            SoulEffects.Add(Effect);
        }

        internal void StoreItem(Drop Holding)
        {
            Inventory.Add(Holding);
        }

        internal void onEnterLevel(Scene Scene)
        {
            if (Holding != null)
            {
                Scene.Add(OldSkullGame.Player.Holding);
                OldSkullGame.Player.Holding.onPickUp(Player);
            }
        }
    }
}
