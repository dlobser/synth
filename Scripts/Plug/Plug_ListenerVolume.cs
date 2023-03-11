using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ON.synth
{
    public class Plug_ListenerVolume : MonoBehaviour
    {
        public Oscillator oscillator;
        public Trigger trigger;

        void Update()
        {
            AudioListener.volume = Synth_Util.GetOscTrigValue(oscillator, trigger);
        }
    }
}