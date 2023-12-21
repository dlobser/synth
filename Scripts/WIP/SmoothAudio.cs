using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ON.synth;
// using ImageTools.Core;

public class SmoothAudio : MonoBehaviour
{
    public float frequency;
    [Range(0,1)]
    public float volume;
    [Range(-1,1)]
    public float pan;

    float volumeCounter = 0;
    float prevVolume;
    float prevFrequency;
    float prevPan;

    [Tooltip("If neither are checked, audiosource's clip is modified")]
    public bool generateTone;
    public bool generateNoise;

[System.Serializable]
    public struct tone{
        public float amplitude;
        public float frequency;
        public float pan;
        public float counter {get;set;}
        public float output {get;set;}
        // public Oscillator amplitudeOscillator;
        // public Oscillator frequencyOscillator;
        // public Oscillator panOscillator;
    }
    [Tooltip("X: amplitude, Y: frequency")]
    
    public tone[] tones;

    float sampleRate;

    PerlinNoise noise;

    bool usingClip;

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        noise = new PerlinNoise(1);
        usingClip = GetComponent<AudioSource>().clip != null;
    }

    void OnAudioFilterRead (float[] data, int channels) {
        for (int i = 0; i < data.Length; i += channels) {
            float fraction = ((float)i/data.Length);
            float panLerp = Mathf.Lerp(prevPan,pan,fraction);
            float frequencyLerp = Mathf.Lerp(prevFrequency,frequency,fraction);
            float volumeLerp = Mathf.Lerp(prevVolume,volume,fraction);
            volumeCounter += (1f/sampleRate)*Mathf.PI*2*frequencyLerp;
            volumeCounter = volumeCounter % (Mathf.PI*2);
            float val =  volumeLerp * (Mathf.Sin( volumeCounter ));
            if(generateNoise)
                val = volumeLerp * ((float)noise.Noise((double)Mathf.Sin(volumeCounter)*10000,(double)Mathf.Cos(volumeCounter)*10000,0)-.5f)*2;// + ((float)noise.Noise((double)Mathf.Sin(volumeCounter)*100,(double)Mathf.Cos(volumeCounter)*100,(double)Mathf.Cos(volumeCounter)*100)-.5f));
            float[] multiVal = MultiTone((float)1/(float)sampleRate);
            data[i] = ((generateTone||generateNoise?1:usingClip?data[i]:1) * val * multiVal[0]) * (channels==2?Mathf.Clamp(Mathf.Abs(1-panLerp),0,1):1);
            if(channels==2)
                data[i+1] =  ((generateTone||generateNoise?1:usingClip?data[i+1]:1) * val * multiVal[1]) * Mathf.Clamp((panLerp+1),0,1);
        }

        prevFrequency = frequency;
        prevVolume = volume;
        prevPan = pan;
    }

    float[] MultiTone(float t){
        float[] value = new float[]{0,0};
        for (int i = 0; i < tones.Length; i++)
        {
            tones[i].counter += t*tones[i].frequency*Mathf.PI*2;
            tones[i].counter = tones[i].counter % (Mathf.PI*2);
            tones[i].output = tones[i].amplitude * (Mathf.Sin( tones[i].counter ));
            value[0] += tones[i].output * Mathf.Clamp(Mathf.Abs(1-tones[i].pan),0,1);
            value[1] += tones[i].output * Mathf.Clamp((tones[i].pan+1),0,1);
        }
        if(tones.Length==0){
            value[0] = 1;
            value[1] = 1;
        }
        return value;
    }
}
