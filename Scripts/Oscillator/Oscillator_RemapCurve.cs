using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    [System.Serializable]
    public class Oscillator_RemapCurve : Oscillator
    {
        public AnimationCurve curve;
        public Oscillator oscillator;
        public Trigger trigger;
        public string info;
        public bool autoUpdate = false;

        void Update()
        {
            if (autoUpdate)
            {
                GetValue();
            }
        }

        public override float GetValue()
        {
            float v = ON.synth.Synth_Util.GetOscTrigValue(oscillator, trigger);
            float sv = curve.Evaluate(v);
            info = sv + "";
            return sv;
        }

        public override float GetValue(float t)
        {
            float o = 0;
            float v = 1;
            if (oscillator != null)
                o = oscillator.GetValue(t);
            if (trigger != null)
                v = trigger.GetValue();
            float sv = curve.Evaluate(o * v);
            info = sv + "";
            return sv;
        }
    }
}
