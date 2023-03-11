using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger_BPMFromHeartBeat : Trigger
    {
        public override float GetValue(){
            value = GetComponent<Trigger_HeartBeat>().bpm;
            return value;
        }
    }
}