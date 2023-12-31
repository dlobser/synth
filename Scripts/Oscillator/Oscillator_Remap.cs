using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    [System.Serializable]
    public class Oscillator_Remap : Oscillator
    {
        public float lowValue;
        public float highValue;
        public Oscillator_LFO oscillator;
        public float value;

        public override float GetValue()
        {
            float inLow = Mathf.Max(oscillator.clampLow,oscillator.trough*oscillator.multiply+oscillator.offset);
            float inHigh = Mathf.Min(oscillator.clampHigh,oscillator.crest*oscillator.multiply+oscillator.offset);
            value = ON.synth.Synth_Util.map(oscillator.GetValue(), inLow, inHigh, lowValue, highValue);
            return lowValue==highValue ? lowValue : value;
        }

        public override float GetValue(float t)
        {
            float inLow = Mathf.Max(oscillator.clampLow,oscillator.trough*oscillator.multiply+oscillator.offset);
            float inHigh = Mathf.Min(oscillator.clampHigh,oscillator.crest*oscillator.multiply+oscillator.offset);
            return lowValue==highValue ? lowValue : ON.synth.Synth_Util.map(oscillator.GetValue(t), inLow, inHigh, lowValue, highValue);
        }
    }
}
