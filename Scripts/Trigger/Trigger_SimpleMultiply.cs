using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_SimpleMultiply : Trigger
    {

        public Trigger triggerA;
        public Trigger triggerB;

        public string outValue;

        public bool autoUpdate = false;

        private void Update() {
            if (autoUpdate) {
                float val = (triggerA != null ? triggerA.GetValue() : 1) * (triggerB != null ? triggerB.GetValue() : 1);
                value = val;
                outValue = val.ToString();
            }
        }

        public override float GetValue(){
            float val = (triggerA!=null?triggerA.value:1) * (triggerB!=null?triggerB.value:1);
            value = val;
            outValue = val.ToString();
            return val;
        }
    }
}