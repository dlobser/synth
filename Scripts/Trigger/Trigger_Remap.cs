using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger_Remap : Trigger
    {
        public Trigger trigger;
        public float inMin;
        public float inMax;
        public float outMin;
        public float outMax;
        public bool clamp;
        public bool smoothStep;

        void Start()
        {

        }

        void Update()
        {
            float v = trigger.value;//smoothStep ? Mathf.SmoothStep(inMin,inMax,trigger.value) : trigger.value;
            if (clamp)
                v = Mathf.Clamp(v, inMin < inMax ? inMin : inMax, inMin < inMax ? inMax : inMin);
            if (smoothStep)
            {
                float s = Mathf.SmoothStep(inMin, inMax, ON.synth.Synth_Util.map(v, inMin, inMax, 0, 1));
                v = s;
            }
            value = ON.synth.Synth_Util.map(v, inMin, inMax, outMin, outMax);

        }
    }
}

