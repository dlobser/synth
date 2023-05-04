using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_AudioChorusFilter : MonoBehaviour
    {
        public AudioChorusFilter effect;
        public Oscillator oscillator_DryMix;
        public Trigger trigger_DryMix;
        public Oscillator oscillator_WetMix;
        public Trigger trigger_WetMix;
        public Oscillator oscillator_Delay;
        public Trigger trigger_Delay;
        public Oscillator oscillator_Rate;
        public Trigger trigger_Rate;
        public Oscillator oscillator_Depth;
        public Trigger trigger_Depth;

        void Update()
        {
            if (oscillator_DryMix != null || trigger_DryMix != null)
            {
                float dry = ON.synth.Synth_Util.GetOscTrigValue(oscillator_DryMix, trigger_DryMix);
                effect.dryMix = dry;
            }
            if (oscillator_WetMix != null || trigger_WetMix != null)
            {
                float wet = ON.synth.Synth_Util.GetOscTrigValue(oscillator_WetMix, trigger_WetMix);
                effect.wetMix1 = wet;
                effect.wetMix2 = wet;
            }
            if (oscillator_Delay != null || trigger_Delay != null)
            {
                float delay = ON.synth.Synth_Util.GetOscTrigValue(oscillator_Delay, trigger_Delay);
                effect.delay = delay;
            }
            if (oscillator_Rate != null || trigger_Rate != null)
            {
                float rate = ON.synth.Synth_Util.GetOscTrigValue(oscillator_Rate, trigger_Rate);
                effect.rate = rate;
            }
            if (oscillator_Depth != null || trigger_Depth != null)
            {
                float depth = ON.synth.Synth_Util.GetOscTrigValue(oscillator_Depth, trigger_Depth);
                effect.depth = depth;
            }


        }

    }
}