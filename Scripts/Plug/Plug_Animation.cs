using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_Animation : MonoBehaviour
    {

        public string parameter;
        public Animator animator;
        public Oscillator oscillator;
        public Trigger trigger;

        void Start()
        {
            animator.SetFloat(parameter, Synth_Util.GetOscTrigValue(oscillator, trigger));
        }

        void Update()
        {
            animator.SetFloat(parameter, Synth_Util.GetOscTrigValue(oscillator, trigger));
        }

    }
}