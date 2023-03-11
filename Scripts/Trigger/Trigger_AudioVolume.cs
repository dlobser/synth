using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_AudioVolume : Trigger {
    
        public AudioSource audioSource;
        public float updateStep = 0.1f;
        public int sampleDataLength = 1024;
    
        private float currentUpdateTime = 0f;
    
        private float clipLoudness;
        private float[] clipSampleData;

        public float low = 0 , high = 1, inLow = 0, inHigh = 1;

        public bool useSourceAudio = false;

        public bool lerpValue;
        public float lerpUpSpeed = 1000;
        public float lerpDownSpeed = 1000;
        float lValue;
    
        // Use this for initialization
        void Awake () {
        
            if (!audioSource) {
                Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
            }
            clipSampleData = new float[sampleDataLength];
    
        }
        
        // Update is called once per frame
        void Update () {
            if(audioSource!=null){
                currentUpdateTime += Time.deltaTime;
                if (currentUpdateTime >= updateStep) {
                    currentUpdateTime = 0f;
                    audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
                    clipLoudness = 0f;
                    foreach (var sample in clipSampleData) {
                        clipLoudness += Mathf.Abs(sample);
                    }
                    
                    clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
                }

                if (lerpValue) {
                    float v = ON.Utils.map(clipLoudness, inLow, inHigh, low, high);
                    v = Mathf.Clamp(v, low, high);
                    if (useSourceAudio)
                        v *= audioSource.volume;
                    if (v > value) {
                        value = Mathf.Lerp(value, v, lerpUpSpeed * Time.deltaTime);
                    }
                    else {
                        value = Mathf.Lerp(value, v, lerpDownSpeed * Time.deltaTime);
                    }
                    lValue = v;

                }

                else {
                    value = ON.Utils.map(clipLoudness, inLow, inHigh, low, high);
                    value = Mathf.Clamp(value, low, high);
                    if (useSourceAudio)
                        value *= audioSource.volume;
                }
                

            }
    // print(value);
        }

        public override float GetValue(){
            return value;
        }
    
    }
 }
