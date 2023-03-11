using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{

    [System.Serializable]
    public class Oscillators
    {
        public Oscillator multiplyOscillate;
        public Oscillator speedOscillate;
        public Oscillator offsetOscillate;
        public Oscillator troughOscillate;
        public Oscillator crestOscillate;
        public Oscillator clampLowOscillate;
        public Oscillator clampHighOscillate;
        public Oscillator timeOffsetOscillate;
    }

    [System.Serializable]
    public class Triggers
    {
        public Trigger multiplyTrigger;
        public Trigger speedTrigger;
        public Trigger offsetTrigger;
        public Trigger troughTrigger;
        public Trigger crestTrigger;
        public Trigger clampLowTrigger;
        public Trigger clampHighTrigger;
        public Trigger timeOffsetTrigger;
    }

    [System.Serializable]
    public class Oscillator : MonoBehaviour
    {
        public virtual float GetValue(){return 1;}
        public virtual float GetValue(float t){return t;}
        public virtual void ResetCounter(){}
    }

}