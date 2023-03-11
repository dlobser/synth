using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ON.synth
{
    public static class Synth_Util
    {
        public static float GetOscValue(Oscillator oscillator){
            float value = 1;
            if (oscillator != null){
                value = oscillator.GetValue();
            }
            return value;
        }

        public static float GetOscTrigValue(Oscillator oscillator, Trigger trigger){
            float value = 1;
            if (oscillator != null){
                value = oscillator.GetValue();
                if (trigger != null)
                    value *= trigger.GetValue();
            }
            else if (trigger != null)
                value = trigger.GetValue() ;
            return value;
        }
    }
}