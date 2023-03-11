using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    [System.Serializable]
    public class Oscillator_SetValue : Oscillator
    {
        public float value;
        public override float GetValue(){
            return value;
        }
        public override float GetValue(float t){
            return value;
        }
    }
}
