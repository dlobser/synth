using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger_Lazy : Trigger
    {

        public Oscillator oscillator;
        public Trigger trigger;
        public float lazySpeed;

        void Update()
        {
            float v = ON.synth.Synth_Util.GetOscTrigValue(oscillator, trigger);
            value = Mathf.Lerp(value, v, Time.deltaTime * lazySpeed);
        }

        public override float GetValue()
        {
            return value;
        }
    }
}