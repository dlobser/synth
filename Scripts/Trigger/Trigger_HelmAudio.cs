using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger_HelmAudio : Trigger
    {
        public float low;
        public float high;
        public float pitchLow;
        public float pitchHigh;
        public float inPitchLow;
        public float inPitchHigh;
        public float pitch;
        bool ison;
        public float decayRate;
        float noteValue;
        public Plug_Helm helm;

        void Start()
        {
            
        }

        void OnEnable(){
            helm.NoteOn += NoteOn;
            helm.NoteOff += NoteOff;
        }

        void OnDisable(){
            helm.NoteOn -= NoteOn;
            helm.NoteOff -= NoteOff;

        }

        void Update()
        {
            if(ison){
                noteValue = 1;
            }
            else{
                noteValue -= Time.deltaTime * ((decayRate<=0)?1:(1/decayRate));
            }
            value = ON.Utils.map(noteValue,0,1,low,high);
            value = Mathf.Clamp(value,low,high);
        }

        public void NoteOn(){
            ison = true;
        }

        public void NoteOff(){
            ison = false;
        }

        public override float GetValue(){
            return value;
        }
    }
}