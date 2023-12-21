using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using ImageTools.Core;
using System;

namespace ON.synth
{
    public class Plug_AudioSynth : MonoBehaviour
    {

        [System.Serializable]
        public struct Oscillators
        {
            public Oscillator volumeOscillator;
            public Oscillator frequencyOscillator;
            public Oscillator panOscillator;
        }

        [System.Serializable]
        public struct tone
        {
            public float volume;
            public float frequency;
            public float pan;
            public bool useNoise;
            public bool useCustomCurve;
            public AnimationCurve curve;
            public int tick {get;set;}
            public double counter { get; set; }
            public double output { get; set; }
            public double time { get; set; }
            public double prevVolume { get; set; }
            public double prevFrequency { get; set; }
            public double prevPan { get; set; }
            public Oscillators oscillators;
            public bool volumeOscillateUpdate { get; set; }
            public bool frequencyOscillateUpdate { get; set; }
            public bool panOscillateUpdate { get; set; }
        }

        [Tooltip("X: amplitude, Y: frequency")]

        public tone[] tones;

        float sampleRate;

        PerlinNoise noise;


        // bool usingClip;

        void Start()
        {

            sampleRate = AudioSettings.outputSampleRate;
            noise = new PerlinNoise(1);
            // usingClip = GetComponent<AudioSource>().clip != null;
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            for (int i = 0; i < tones.Length; i++)
            {

                // if(tones[i].volumeOscillator!=null)
                //     tones[i].volume = tones[i].volumeOscillator.GetValue(tones[i].time);
                // if(tones[i].frequencyOscillator!=null)
                //     tones[i].frequency = tones[i].frequencyOscillator.GetValue(tones[i].time);
                // if(tones[i].panOscillator!=null)
                //     tones[i].pan = tones[i].panOscillator.GetValue(tones[i].time);
                tones[i].volumeOscillateUpdate = false;
                tones[i].frequencyOscillateUpdate = false;
                tones[i].panOscillateUpdate = false;
            }

            for (int i = 0; i < data.Length; i += channels)
            {
                float fraction = ((float)i / data.Length);
                double[] multiVal = MultiTone((double)1 / (double)sampleRate, fraction);
                data[i] = (float)multiVal[0];// * (channels==2?Mathf.Clamp(Mathf.Abs(1-panLerp),0,1):1);
                if (channels == 2)
                    data[i + 1] = (float)multiVal[1];//((generateTone||generateNoise?1:usingClip?data[i+1]:1) * val * multiVal[1]) * Mathf.Clamp((panLerp+1),0,1);
            }

            //  print(tones[0].prevVolume + " | " + tones[0].volume);

            for (int i = 0; i < tones.Length; i++)
            {
                // tones[i].time += ((float)data.Length/(float)sampleRate); 
                // tones[i].time = tones[i].time % (Mathf.PI*2);
                tones[i].prevVolume = tones[i].volume;
                tones[i].prevFrequency = tones[i].frequency;
                tones[i].prevPan = tones[i].pan;
                tones[i].volumeOscillateUpdate = true;
                tones[i].frequencyOscillateUpdate = true;
                tones[i].panOscillateUpdate = true;

            }

            // prevFrequency = frequency;
            // prevVolume = volume;
            // prevPan = pan;
        }

        void Update()
        {
            for (int i = 0; i < tones.Length; i++)
            {

                if (tones[i].oscillators.volumeOscillator != null && tones[i].volumeOscillateUpdate)
                    tones[i].volume = Synth_Util.GetOscValue(tones[i].oscillators.volumeOscillator);
                if (tones[i].oscillators.frequencyOscillator != null && tones[i].frequencyOscillateUpdate)
                    tones[i].frequency = Synth_Util.GetOscValue(tones[i].oscillators.frequencyOscillator);
                if (tones[i].oscillators.panOscillator != null && tones[i].panOscillateUpdate)
                    tones[i].pan = Synth_Util.GetOscValue(tones[i].oscillators.panOscillator);

            }
        }
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }


        

        double[] MultiTone(double t, double fraction)
        {

            double[] value = new double[] { 0, 0 };

            for (int i = 0; i < tones.Length; i++)
            {
                double pan =         Lerp(tones[i].prevPan, tones[i].pan, fraction);
                double volume =      Lerp(tones[i].prevVolume, tones[i].volume, fraction);
                double frequency =       Lerp(tones[i].prevFrequency, tones[i].frequency,fraction);
                tones[i].tick++;
                
                tones[i].counter += t * (tones[i].useNoise?frequency*0.0001:frequency) * Mathf.PI * 2;
                if(!tones[i].useNoise)
                    tones[i].counter = tones[i].counter % (Mathf.PI * 2);
                else
                    tones[i].counter = tones[i].counter % 1e7;
                if(tones[i].useNoise){
                    double dn = noise.Noise(System.Math.Sin(tones[i].counter) * 100000,System.Math.Cos(tones[i].counter) * 100000, tones[i].counter);
                    tones[i].output = Lerp(tones[i].output, dn, 0.5) * volume;//Mathf.Sin((float)tones[i].counter) * 100000, Mathf.Cos((float)tones[i].counter) * 100000) - 0.5f;
                }
                    //(double)Mathf.Lerp((float)tones[i].output,(float)volume * (Mathf.PerlinNoise(Mathf.Sin((float)tones[i].counter) * 100000,Mathf.Cos((float)tones[i].counter) * 100000)-.5f)*2,.5f);
                else if(tones[i].useCustomCurve)
                    tones[i].output = volume * tones[i].curve.Evaluate((float)(tones[i].counter / (Math.PI * 2.0)));
                else
                   tones[i].output = volume * System.Math.Sin(tones[i].counter);


                value[0] += tones[i].output * System.Math.Clamp(System.Math.Abs(1.0 - pan), 0.0f, 1.0);
                value[1] += tones[i].output * System.Math.Clamp(pan + 1.0, 0.0f, 1.0f);

            }

            if (tones.Length == 0)
            {
                value[0] = 1;
                value[1] = 1;
            }

            return value;

        }
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// // using ImageTools.Core;

// namespace ON.synth
// {
//     public class Plug_AudioSynth : MonoBehaviour
//     {

//         [System.Serializable]
//         public struct Oscillators
//         {
//             public Oscillator volumeOscillator;
//             public Oscillator frequencyOscillator;
//             public Oscillator panOscillator;
//         }

//         [System.Serializable]
//         public struct tone
//         {
//             public float volume;
//             public float frequency;
//             public float pan;
//             public float counter { get; set; }
//             public float output { get; set; }
//             public float time { get; set; }
//             public float prevVolume { get; set; }
//             public float prevFrequency { get; set; }
//             public float prevPan { get; set; }
//             public Oscillators oscillators;
//             public bool volumeOscillateUpdate { get; set; }
//             public bool frequencyOscillateUpdate { get; set; }
//             public bool panOscillateUpdate { get; set; }
//         }

//         [Tooltip("X: amplitude, Y: frequency")]

//         public tone[] tones;
//         AudioListener listener;
//         float sampleRate;
//         Vector3 listenerPos;
//         Vector3 sourcePos;
//         Vector3 relativePos;
//         float stereoPan;
//         float prevStereoPan;
//         float[] value;

//         // PerlinNoise noise;

//         // bool usingClip;

//         void Start()
//         {
//             value = new float[] { 0, 0 };
//             sampleRate = AudioSettings.outputSampleRate;
//             // noise = new PerlinNoise(1);
//             // usingClip = GetComponent<AudioSource>().clip != null;
//         }

//         // void OnAudioFilterRead(float[] data, int channels)
//         // {
           
//         //     for (int i = 0; i < tones.Length; i++)
//         //     {

//         //         // if(tones[i].volumeOscillator!=null)
//         //         //     tones[i].volume = tones[i].volumeOscillator.GetValue(tones[i].time);
//         //         // if(tones[i].frequencyOscillator!=null)
//         //         //     tones[i].frequency = tones[i].frequencyOscillator.GetValue(tones[i].time);
//         //         // if(tones[i].panOscillator!=null)
//         //         //     tones[i].pan = tones[i].panOscillator.GetValue(tones[i].time);
//         //         tones[i].volumeOscillateUpdate = false;
//         //         tones[i].frequencyOscillateUpdate = false;
//         //         tones[i].panOscillateUpdate = false;
//         //     }
//         //     for (int i = 0; i < data.Length; i += channels)
//         //     {
//         //         float fraction = ((float)i / data.Length);
//         //         // float sPan = Mathf.Lerp(prevStereoPan, stereoPan, fraction);
//         //         float[] multiVal = MultiTone((float)1 / (float)sampleRate, fraction);
                
//         //         // Calculate the final pan value by combining static pan and stereo pan
//         //         // float finalPan = Mathf.Clamp(sPan, -1f, 1f);
                
//         //         // // Calculate left and right volumes based on finalPan
//         //         // float leftVolume = finalPan <= 0 ? 1f : 1f - finalPan;
//         //         // float rightVolume = finalPan >= 0 ? 1f : 1f + finalPan;

//         //         // Apply the calculated volumes to the audio data
//         //         data[i] = multiVal[0];// * leftVolume;
//         //         if (channels == 2)
//         //             data[i + 1] = multiVal[1];// * rightVolume;
//         //     }

//         //     //  print(tones[0].prevVolume + " | " + tones[0].volume);

//         //     for (int i = 0; i < tones.Length; i++)
//         //     {
//         //         // tones[i].time += ((float)data.Length/(float)sampleRate); 
//         //         // tones[i].time = tones[i].time % (Mathf.PI*2);
//         //         tones[i].prevVolume = tones[i].volume;
//         //         tones[i].prevFrequency = tones[i].frequency;
//         //         tones[i].prevPan = tones[i].pan;
//         //         tones[i].volumeOscillateUpdate = true;
//         //         tones[i].frequencyOscillateUpdate = true;
//         //         tones[i].panOscillateUpdate = true;

//         //     }

//         //     // prevFrequency = frequency;
//         //     // prevVolume = volume;
//         //     // prevPan = pan;
//         // }

//         void OnAudioFilterRead(float[] data, int channels)
//         {
//             double startTime = AudioSettings.dspTime;
//             double sampleDuration = 1.0 / sampleRate;

//             for (int i = 0; i < data.Length; i += channels)
//             {
//                 // Calculate the exact time for this sample
//                 double time = startTime + i / channels * sampleDuration;
//                 float fraction = (float)(i / (double)data.Length);

//                 // Update the tones time based on dspTime
//                 for (int j = 0; j < tones.Length; j++)
//                 {
//                     tones[j].time = (float)time;
//                     // Update other properties if needed
//                 }
//                 value[0] = 0;
//                 value[1] = 0;
//                 // float[] multiVal = value;//
//                 MultiTone((float)sampleDuration, fraction);
                
//                 data[i] = value[i % 2];
//                 if (channels == 2)
//                     data[i + 1] = value[(i + 1) % 2];
//             }

//             // Update previous values for tones
//             for (int i = 0; i < tones.Length; i++)
//             {
//                 tones[i].prevVolume = tones[i].volume;
//                 tones[i].prevFrequency = tones[i].frequency;
//                 tones[i].prevPan = tones[i].pan;
//                 tones[i].volumeOscillateUpdate = true;
//                 tones[i].frequencyOscillateUpdate = true;
//                 tones[i].panOscillateUpdate = true;
//             }
//         }



//         void Update()
//         {
//             if (listener == null)
//                 listener = FindObjectOfType<AudioListener>();

//             prevStereoPan = stereoPan;
//             listenerPos = listener.transform.position;
//             sourcePos = transform.position;
//             relativePos = listener.transform.InverseTransformPoint(sourcePos);
//             stereoPan = Mathf.Clamp(relativePos.x / Mathf.Abs(relativePos.x), -1f, 1f);
//             float distance = Vector3.Distance(listenerPos, sourcePos);
//             stereoPan = distance>0?Mathf.Lerp(0,stereoPan,distance/10f):0;

//             for (int i = 0; i < tones.Length; i++)
//             {

//                 if (tones[i].oscillators.volumeOscillator != null && tones[i].volumeOscillateUpdate)
//                     tones[i].volume = Synth_Util.GetOscValue(tones[i].oscillators.volumeOscillator);
//                 if (tones[i].oscillators.frequencyOscillator != null && tones[i].frequencyOscillateUpdate)
//                     tones[i].frequency = Synth_Util.GetOscValue(tones[i].oscillators.frequencyOscillator);
//                 if (tones[i].oscillators.panOscillator != null && tones[i].panOscillateUpdate)
//                     tones[i].pan = Synth_Util.GetOscValue(tones[i].oscillators.panOscillator) + stereoPan;
//             }
//         }

//         void MultiTone(float t, float fraction)
//         {

            

//             for (int i = 0; i < tones.Length; i++)
//             {
//                 float pan = Mathf.Lerp(tones[i].prevPan, tones[i].pan , fraction);
//                 float volume = Mathf.Lerp(tones[i].prevVolume, tones[i].volume, fraction);
//                 float frequency = Mathf.Lerp(tones[i].prevFrequency, tones[i].frequency, fraction);

//                 tones[i].counter += t * frequency * Mathf.PI * 2;
//                 tones[i].counter = tones[i].counter % (Mathf.PI * 2);
//                 tones[i].output = volume * (Mathf.Sin(tones[i].counter));

//                 value[0] += tones[i].output * Mathf.Clamp(Mathf.Abs(1 - pan), 0, 1);
//                 value[1] += tones[i].output * Mathf.Clamp((pan + 1), 0, 1);
//             }

//             if (tones.Length == 0)
//             {
//                 value[0] = 1;
//                 value[1] = 1;
//             }

//             // return value;

//         }
//     }
// }