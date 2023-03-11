using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Samples oscillators at speeds greater than the framerate
//Speed needs to be handled in a special way, use speed oscillators here instead of on the oscillator
//It is not possible to modify pitch with this method 

namespace ON.synth{

    public class Plug_AudioSmooth : MonoBehaviour
    {

        public AudioSource audio;
        public Oscillator volumeOscillator;
        public Oscillator panOscillator;
        public Trigger volumeTrigger;
        public Trigger panTrigger;

        public Oscillator volumeSpeedOscillator;
        float vSpeed;

        public Oscillator panSpeedOscillator;
        float pSpeed;

        public Conductor conductor;
        [Range(0,1)]
        public float volume;
        [Range(-1,1)]
        public float pan;

        float prevVolume;
        float prevPan;

        float oVolume;
        float tVolume;
        float prevtVolume;
        float cVolume;
        float prevcVolume;
        float oPan;
        float vSpeedCounter;
        float pSpeedCounter;
        float sCounter;

        float sampleRate;

        public bool generateTone = false;

        void Start()
        {
            sampleRate = AudioSettings.outputSampleRate;
        }

        void Update()
        {
            if (volumeTrigger != null)
                tVolume = volumeTrigger.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
            else
                tVolume = 1;

            if(conductor!=null)
                cVolume = conductor.masterVolume;
            else
                cVolume = 1;
        }

        void OnAudioFilterRead (float[] data, int channels) {

            for (int i = 0; i < data.Length; i += channels) {  

                if (volumeSpeedOscillator != null){
                    vSpeed = volumeSpeedOscillator.GetValue(sCounter);// * ((conductor!=null)?conductor.masterSpe:1);
                }
                else
                    vSpeed = 1;
                
                if (panSpeedOscillator != null){
                    pSpeed = panSpeedOscillator.GetValue(sCounter);// * ((conductor!=null)?conductor.masterVolume:1);
                }
                else
                    pSpeed = 1;

                sCounter += (1f/sampleRate);
                vSpeedCounter += (1f/sampleRate) * ((vSpeed!=0) ? (1/vSpeed) : 0);//(float)data.Length);
                pSpeedCounter += (1f/sampleRate) * ((pSpeed!=0) ? (1/pSpeed) : 0);

                sCounter = sCounter % (Mathf.PI*2*1000);
                vSpeedCounter = vSpeedCounter % (Mathf.PI*2*1000);
                pSpeedCounter = pSpeedCounter % (Mathf.PI*2*1000);

                float fraction = ((float)i/(float)data.Length );
                float panLerp = Mathf.Lerp(prevPan,pan,fraction);
                float volumeLerp = Mathf.Lerp(prevVolume,volume,fraction);
                float tVolumeLerp = Mathf.Lerp(prevtVolume, tVolume, fraction);
                float cVolumeLerp = Mathf.Lerp(prevcVolume,cVolume, fraction);

                if (volumeOscillator != null){
                    oVolume = volumeOscillator.GetValue(vSpeedCounter);
                }
                else
                    oVolume = 1;
                
                float finalVolume = volumeLerp * oVolume * cVolumeLerp * tVolumeLerp * ((conductor!=null)?conductor.masterVolume:1);
                
                if (panOscillator != null){
                    oPan = panOscillator.GetValue(pSpeedCounter);
                    if (panTrigger != null)
                        oPan *= panTrigger.GetValue();
                }
                else if (panTrigger != null)
                    oPan = panTrigger.GetValue() ;
                else
                    oPan = panLerp;


                data[i] =  finalVolume * (generateTone?1:data[i]) * (channels==2?Mathf.Clamp(Mathf.Abs(1-oPan),0,1):1);
                if(channels==2)
                    data[i+1] = finalVolume *  (generateTone?1:data[i+1])  * Mathf.Clamp((oPan+1),0,1);
            }
            prevVolume = volume;
            prevcVolume = cVolume;
            prevPan = pan;
            prevtVolume = tVolume;
        }
    }
}