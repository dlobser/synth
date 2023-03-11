using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger : MonoBehaviour
    {
        public float value;
        public bool debug;
        public virtual float GetValue(){if(debug)print(this.gameObject.name + " Trigger Value: " + value); return value; }
    }
}