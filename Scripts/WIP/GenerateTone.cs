using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using ImageTools.Core;
using System;
using ON.synth;

// namespace ON.synth
// {
    public class GenerateTone : MonoBehaviour
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
        float sampleRate;

        PerlinNoise noise;

        void Start()
        {
            sampleRate = AudioSettings.outputSampleRate;
            noise = new PerlinNoise(1);
        }

        void OnAudioFilterRead(float[] data, int channels)
        {

            for (int i = 0; i < data.Length; i += channels)
            {
                float fraction = ((float)i / data.Length);
                double[] multiVal = MultiTone((double)1 / (double)sampleRate, fraction);
                data[i] = (float)multiVal[0];
                if (channels == 2)
                    data[i + 1] = (float)multiVal[1];
            }

            prevVolume = volume;
            prevFrequency = frequency;
            prevPan = pan;
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        double[] MultiTone(double t, double fraction)
        {

            double[] value = new double[] { 0, 0 };

            double _pan =         Lerp(prevPan, pan, fraction);
            double _volume =      Lerp(prevVolume, volume, fraction);
            double _frequency =       Lerp(prevFrequency, frequency,fraction);
            tick++;
            
            counter += t * (useNoise?_frequency*0.0001:_frequency) * Mathf.PI * 2;
            if(!useNoise)
                counter = counter % (Mathf.PI * 2);
            else
                counter = counter % 1e7;
            if(useNoise){
                double dn = noise.Noise(System.Math.Sin(counter) * 100000,System.Math.Cos(counter) * 100000, counter);
                output = Lerp(output, dn, 0.5) * _volume;
            }
            else if(useCustomCurve)
                output = _volume * curve.Evaluate((float)(counter / (Math.PI * 2.0)));
            else
                output = _volume * System.Math.Sin(counter);

            value[0] += output * System.Math.Clamp(System.Math.Abs(1.0 - _pan), 0.0f, 1.0);
            value[1] += output * System.Math.Clamp(_pan + 1.0, 0.0f, 1.0f);

            return value;

        }
    }
// }
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

//         //         // if(volumeOscillator!=null)
//         //         //     volume = volumeOscillator.GetValue(time);
//         //         // if(frequencyOscillator!=null)
//         //         //     frequency = frequencyOscillator.GetValue(time);
//         //         // if(panOscillator!=null)
//         //         //     pan = panOscillator.GetValue(time);
//         //         volumeOscillateUpdate = false;
//         //         frequencyOscillateUpdate = false;
//         //         panOscillateUpdate = false;
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
//         //         // time += ((float)data.Length/(float)sampleRate); 
//         //         // time = time % (Mathf.PI*2);
//         //         prevVolume = volume;
//         //         prevFrequency = frequency;
//         //         prevPan = pan;
//         //         volumeOscillateUpdate = true;
//         //         frequencyOscillateUpdate = true;
//         //         panOscillateUpdate = true;

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
//                 prevVolume = volume;
//                 prevFrequency = frequency;
//                 prevPan = pan;
//                 volumeOscillateUpdate = true;
//                 frequencyOscillateUpdate = true;
//                 panOscillateUpdate = true;
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

//                 if (oscillators.volumeOscillator != null && volumeOscillateUpdate)
//                     volume = Synth_Util.GetOscValue(oscillators.volumeOscillator);
//                 if (oscillators.frequencyOscillator != null && frequencyOscillateUpdate)
//                     frequency = Synth_Util.GetOscValue(oscillators.frequencyOscillator);
//                 if (oscillators.panOscillator != null && panOscillateUpdate)
//                     pan = Synth_Util.GetOscValue(oscillators.panOscillator) + stereoPan;
//             }
//         }

//         void MultiTone(float t, float fraction)
//         {

            

//             for (int i = 0; i < tones.Length; i++)
//             {
//                 float pan = Mathf.Lerp(prevPan, pan , fraction);
//                 float volume = Mathf.Lerp(prevVolume, volume, fraction);
//                 float frequency = Mathf.Lerp(prevFrequency, frequency, fraction);

//                 counter += t * frequency * Mathf.PI * 2;
//                 counter = counter % (Mathf.PI * 2);
//                 output = volume * (Mathf.Sin(counter));

//                 value[0] += output * Mathf.Clamp(Mathf.Abs(1 - pan), 0, 1);
//                 value[1] += output * Mathf.Clamp((pan + 1), 0, 1);
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