using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OldSkull.Isle
{
    internal class PlayerStatEffect
    {

        public int Duration=0;
        public float Increment=0;
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
