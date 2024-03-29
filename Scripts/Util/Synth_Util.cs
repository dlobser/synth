﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ON.synth
{
    public static class Synth_Util
    {
        public static float GetOscValue(Oscillator oscillator)
        {
            float value = 1;
            if (oscillator != null)
            {
                value = oscillator.GetValue();
            }
            return value;
        }

        public static float GetOscTrigValue(Oscillator oscillator, Trigger trigger, float val = 1)
        {
            float value = val;
            if (oscillator != null)
            {
                value = oscillator.GetValue();
                if (trigger != null)
                    value *= trigger.GetValue();
            }
            else if (trigger != null)
                value = trigger.GetValue();
            return value;
        }
        
        public static float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }
    }
}