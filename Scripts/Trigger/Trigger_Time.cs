using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_Time : Trigger
    {
        public float speed;
        float counter;

        void Update()
        {
            counter += Time.deltaTime*speed;
        }

        public override float GetValue(){
            value = counter;
            return value;
        }
    }
}