using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_AudioReverbFilter : MonoBehaviour
    {
        public AudioReverbFilter effect;
        public Oscillator oscillator;
        public Trigger trigger;

        void Update()
        {
            float value = ON.synth.Synth_Util.GetOscTrigValue(oscillator, trigger);
            effect.dryLevel = value;
        }

    }
}