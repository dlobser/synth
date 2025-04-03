using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    [System.Serializable]
    public class Oscillator_RemapSetValues : Oscillator
    {
        public Oscillator oscillator;

        public float inLowValue;
        public Oscillator inLowOscillator;
        public float inHightValue;
        public Oscillator inHighOscillator;
        public float lowValue;
        public Oscillator lowOscillator;
        public float highValue;
        public Oscillator highOscillator;
        public float value;

        public override float GetValue()
        {
            float inLow = inLowOscillator!=null?inLowOscillator.GetValue():inLowValue;
            float inHigh = inHighOscillator!=null?inHighOscillator.GetValue():inHightValue;
            float low = lowOscillator != null ? lowOscillator.GetValue() : lowValue;
            float high = highOscillator != null ? highOscillator.GetValue() : highValue;
            value = ON.synth.Synth_Util.map(oscillator.GetValue(), inLow, inHigh, low, high);
            return lowValue==highValue ? lowValue : value;
        }

        public override float GetValue(float t)
        {
            float inLow = inLowOscillator!=null?inLowOscillator.GetValue(t):inLowValue;
            float inHigh = inHighOscillator!=null?inHighOscillator.GetValue(t):inHightValue;
            return lowValue==highValue ? lowValue : ON.synth.Synth_Util.map(oscillator.GetValue(t), inLow, inHigh, lowValue, highValue);
        }
    }
}
