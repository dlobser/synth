using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ON.synth
{
    [ExecuteAlways]
    public class Plug_ShaderVariableWOffset : MonoBehaviour
    {
        public Oscillator oscillator;
        public Trigger trigger;
        // public float upSpeed;
        // public float downSpeed;
        float counter;
        float prevValue;
        public Material mat;
        public string channel;
        public bool useOffset = true;

        void Update()
        {
            float value = Synth_Util.GetOscTrigValue(oscillator, trigger);
            counter += Time.deltaTime * value;//* (value > prevValue ? upSpeed : downSpeed);
            prevValue = value;
            mat.SetFloat(channel, useOffset ? counter : value);
        }
    }
}