using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull;
using OldSkull.GameLevel;
using OldSkull.Isle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class PlayerStats
    {
        private Player Player;
        public float Body = 1;
        public float Soul = 1;

        private List<PlayerStatEffect> BodyEffects;
        private List<PlayerStatEffect> SoulEffects;
        
        public enum Attribute { Body, Soul };

        public PlayerStats(Player Player)
        {
            this.Player = Player;

        }

        internal void InitPlayer(GameLevel.Player Player)
        {
            this.Player = Player;
            BodyEffects = new List<PlayerStatEffect>();
            SoulEffects = new List<PlayerStatEffect>();
        }

        public void Update()
        {
            Body -= 0.0001f;

            for (int i = BodyEffects.Count; i > 0; i--)
            {
                Body += BodyEffects[i-1].Increment/1000;

                BodyEffects[i - 1].AdvanceTime();
                if (BodyEffects[i - 1].Exausted)
                {
                    BodyEffects.RemoveAt(i - 1);
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
    }
}
