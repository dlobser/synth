using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_AudioLowPassFilter : MonoBehaviour
    {

        public AudioLowPassFilter lowPass;
        public Oscillator oscillator;
        public Trigger trigger;

        void Update()
        {
            float value = ON.synth.Synth_Util.GetOscTrigValue(oscillator, trigger);
            lowPass.cutoffFrequency = value;
        }

    }
}