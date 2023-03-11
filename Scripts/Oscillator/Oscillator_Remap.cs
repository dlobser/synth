using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    [System.Serializable]
    public class Oscillator_Remap : Oscillator
    {
        public float lowValue;
        public float highValue;
        public Oscillator_LFO oscillator;

        public override float GetValue(){
            return ON.Utils.map(oscillator.GetValue(),oscillator.clampLow,oscillator.clampHigh,lowValue,highValue);
        }
        public override float GetValue(float t){
            return ON.Utils.map(oscillator.GetValue(t),oscillator.clampLow,oscillator.clampHigh,lowValue,highValue);
        }
    }
}
