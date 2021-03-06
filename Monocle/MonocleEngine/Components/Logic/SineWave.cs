﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Monocle
{
    public class SineWave : Component
    {
        /*
         *     SINE WAVE:
         * 
         *  1       x      
         *  |    x     x   
         *  |  x         x 
         *  | x           x
         *  -x-------------x-------------x
         *  |               x           x
         *  |                x         x
         *  |                  x     x
         * -1                     x
         * 
         *     COS WAVE:
         * 
         *  1x                           x
         *  |   x                     x
         *  |     x                 x
         *  |      x               x
         *  --------x-------------x-------
         *  |        x           x
         *  |         x         x
         *  |           x     x
         * -1              x
         * 
         */

        public float Rate = MathHelper.Pi / 64;
        public float Value { get; private set; }
        public float ValueOverTwo { get; private set; }
        public float TwoValue { get; private set; }
        public float Counter { get; private set; }

        public SineWave()
            : base(true, false)
        {

        }

        public SineWave(float rate)
            : this()
        {
            Rate = rate;
        }

        public SineWave(int framesPerWave)
            : this()
        {
            SetFrames(framesPerWave);
        }

        public override void Update()
        {
            Counter = (Counter + Rate) % (MathHelper.TwoPi * 4);
            Value = (float)Math.Sin(Counter);
            ValueOverTwo = (float)Math.Sin(Counter / 2);
            TwoValue = (float)Math.Sin(Counter * 2);
        }

        public void Restart()
        {
            Counter = Value = ValueOverTwo = TwoValue = 0;
        }

        public void SetFrames(int framesPerWave)
        {
            Rate = MathHelper.TwoPi / (float)framesPerWave;
        }

        public float ValueOffset(float offset)
        {
            return (float)Math.Sin(Counter + offset);
        }

        public void Randomize()
        {
            Counter = Calc.Random.NextFloat() * MathHelper.TwoPi * 2;
        }

        public void StartUp()
        {
            Counter = MathHelper.PiOver2;
            Value = (float)Math.Sin(Counter);
            ValueOverTwo = (float)Math.Sin(Counter / 2);
            TwoValue = (float)Math.Sin(Counter * 2);
        }

        public void StartDown()
        {
            Counter = MathHelper.PiOver2 * 3f;
            Value = (float)Math.Sin(Counter);
            ValueOverTwo = (float)Math.Sin(Counter / 2);
            TwoValue = (float)Math.Sin(Counter * 2);
        }
    }
}
