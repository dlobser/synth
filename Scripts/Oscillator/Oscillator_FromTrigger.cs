using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    [System.Serializable]
    public class Oscillator_FromTrigger : Oscillator
    {

        public Trigger trigger;
        public string outValue;

        public override float GetValue()
        {
            outValue = trigger.GetValue() + "";
            return trigger.GetValue();
        }
        public override float GetValue(float t)
        {
            outValue = trigger.GetValue() + "";
            return trigger.GetValue();
        }
    }
}
