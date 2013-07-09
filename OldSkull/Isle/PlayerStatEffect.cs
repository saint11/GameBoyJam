using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OldSkull.Isle
{
    internal class PlayerStatEffect
    {

        public int Duration;
        public float Increment;
        public bool Exausted { get { return (this.Duration <= 0); } }

        public void AdvanceTime()
        {
            this.Duration--;
        }
        internal PlayerStatEffect()
        {

        }
    }
}
