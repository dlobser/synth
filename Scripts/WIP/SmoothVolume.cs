using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothVolume : MonoBehaviour
{

    [Range(0,1)]
    public float volume;

    float volumeCounter = 0;
    float prevVolume;

    void OnAudioFilterRead (float[] data, int channels) {

        for (int i = 0; i < data.Length; i += channels) {
            float fraction = ((float)i/data.Length);
            float val = Mathf.Lerp(prevVolume,volume,fraction);
            data[i] = data[i] * val;
            if(channels==2)
                data[i+1] =  data[i+1] * val;
        }

        prevVolume = volume;

    }


    // void OnAudioFilterRead (float[] data, int channels) {
    //     for (int i = 0; i < data.Length; i += channels) {
    //         float fraction = ((float)i/data.Length);
    //         float panLerp = Mathf.Lerp(prevPan,pan,fraction);
    //         float frequencyLerp = Mathf.Lerp(prevFrequency,frequency,fraction);
    //         float volumeLerp = Mathf.Lerp(prevVolume,volume,fraction);
    //         volumeCounter += (1f/(float)data.Length)*frequencyLerp;
    //         volumeCounter = volumeCounter % (Mathf.PI*2);
    //         float val =  volumeLerp * (Mathf.Sin( volumeCounter ));
    //         float multiVal = MultiTone(fraction);
    //         data[i] = ((generateTone?1:data[i]) * val *  MultiTone(fraction)) * (channels==2?Mathf.Clamp(Mathf.Abs(1-panLerp),0,1):1);
    //         if(channels==2)
    //             data[i+1] =  ((generateTone?1:data[i+1]) * val *  MultiTone(fraction)) * Mathf.Clamp((panLerp+1),0,1);
    //     }
    //     prevFrequency = frequency;
    //     prevVolume = volume;
    //     prevPan = pan;
    // }

    // float MultiTone(float t){
    //     float value = 0;
    //     for (int i = 0; i < tones.Length; i++)
    //     {
    //         tones[i].z += tones[i].x;
    //         tones[i].z = tones[i].z % (Mathf.PI*2);
    //         tones[i].w = tones[i].y * (Mathf.Sin( tones[i].z ));
    //         value *=  tones[i].w;
    //     }
    //     return value;
    // }
}
