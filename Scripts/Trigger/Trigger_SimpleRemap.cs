using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_SimpleRemap : Trigger
    {
        public float low;
        public float high;

        public override float GetValue(){
            return ON.Utils.map(value,0,1,low,high);
        }
    }
}