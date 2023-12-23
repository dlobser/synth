using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using ImageTools.Core;
using System;

namespace ON.synth
{
    [RequireComponent(typeof(AudioSource))]
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
            public double counter { get; set; }
            public double output { get; set; }
            public double time { get; set; }
            public double prevVolume { get; set; }
            public double prevFrequency { get; set; }
            public double prevPan { get; set; }
            public Advanced advanced;
            public bool volumeOscillateUpdate { get; set; }
            public bool frequencyOscillateUpdate { get; set; }
            public bool panOscillateUpdate { get; set; }
        }

        [System.Serializable]
        public struct Advanced
        {
            public float doppler;
            public bool useNoise;
            public bool useCustomCurve;
            public AnimationCurve curve;
            public Oscillators oscillators;
        }

        [Tooltip("X: amplitude, Y: frequency")]

        public tone[] tones;

        float sampleRate;

        PerlinNoise noise;

        float stereoPan;
        float prevStereoPan;
        float stereoPanLerp;

        float doppler;
        float prevDoppler;
        float dopplerLerp;

        AudioSource audioSource;

        private Vector3 previousSourcePosition;
        private Vector3 previousListenerPosition;

        void Start()
        {
            sampleRate = AudioSettings.outputSampleRate;
            noise = new PerlinNoise(1);
            audioSource = GetComponent<AudioSource>();
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if(tones!=null){
                if(tones.Length>0){
                    for (int i = 0; i < tones.Length; i++)
                    {
                        tones[i].volumeOscillateUpdate = false;
                        tones[i].frequencyOscillateUpdate = false;
                        tones[i].panOscillateUpdate = false;
                    }

                    for (int i = 0; i < data.Length; i += channels)
                    {
                        float fraction = ((float)i / data.Length);
                        double[] multiVal = MultiTone((double)1 / (double)sampleRate, fraction);
                        stereoPanLerp = Mathf.Lerp(prevStereoPan, stereoPan, fraction);
                        dopplerLerp = Mathf.Lerp(prevDoppler, doppler, fraction);
                        data[i] = (float)multiVal[0];
                        if (channels == 2)
                            data[i + 1] = (float)multiVal[1];
                    }

                    for (int i = 0; i < tones.Length; i++)
                    {
                        tones[i].prevVolume = tones[i].volume;
                        tones[i].prevFrequency = tones[i].frequency;
                        tones[i].prevPan = tones[i].pan;
                        tones[i].volumeOscillateUpdate = true;
                        tones[i].frequencyOscillateUpdate = true;
                        tones[i].panOscillateUpdate = true;
                    }
                    prevDoppler = doppler;
                    prevStereoPan = stereoPan;
                }
            }
        }
        
        void Update()
        {
            if(audioSource.spatialBlend!=0)
                stereoPan = Mathf.Lerp(0,CalculateStereoPan(transform.position, Camera.main.transform),audioSource.spatialBlend);

            doppler = CalculateDopplerShift(this.transform.position,Camera.main.transform.position,0);
                
            for (int i = 0; i < tones.Length; i++)
            {

                if (tones[i].advanced.oscillators.volumeOscillator != null && tones[i].volumeOscillateUpdate)
                    tones[i].volume = Synth_Util.GetOscValue(tones[i].advanced.oscillators.volumeOscillator);
                if (tones[i].advanced.oscillators.frequencyOscillator != null && tones[i].frequencyOscillateUpdate)
                    tones[i].frequency = Synth_Util.GetOscValue(tones[i].advanced.oscillators.frequencyOscillator);
                if (tones[i].advanced.oscillators.panOscillator != null && tones[i].panOscillateUpdate)
                    tones[i].pan = Synth_Util.GetOscValue(tones[i].advanced.oscillators.panOscillator);
            }
        }

        public float CalculateStereoPan(Vector3 objectPosition, Transform listenerTransform)
        {
            // Get the vector from the listener to the object
            Vector3 directionToObject = (objectPosition - listenerTransform.position).normalized;

            // Calculate the dot product between the listener's right vector and the direction to the object
            // This gives us a value between -1 and 1, indicating the direction and extent of panning
            float pan = Vector3.Dot(listenerTransform.right, directionToObject);

            // Clamp the pan value to ensure it's within the -1 to 1 range
            pan = Mathf.Clamp(pan, -1f, 1f);

            return pan;
        }

        private const float SpeedOfSound = 343.0f; // Speed of sound in m/s

        public float CalculateDopplerShift(Vector3 sourcePosition, 
                                                Vector3 listenerPosition,
                                                float originalFrequency)
        {

            Vector3 sourceVelocity = (sourcePosition - previousSourcePosition) / Time.deltaTime;
            Vector3 listenerVelocity = (listenerPosition - previousListenerPosition) / Time.deltaTime;

            // Calculate relative velocity
            Vector3 relativeVelocity = listenerVelocity - sourceVelocity;

            // Calculate the velocity of the listener relative to the source
            float listenerRelativeVelocity = Vector3.Dot(relativeVelocity.normalized, (listenerPosition - sourcePosition).normalized);
            
            // Calculate the velocity of the source relative to the listener
            float sourceRelativeVelocity = -Vector3.Dot(sourceVelocity.normalized, (sourcePosition - listenerPosition).normalized);

            // Calculate the perceived frequency
            // float perceivedFrequency = originalFrequency * ((SpeedOfSound + listenerRelativeVelocity) / (SpeedOfSound + sourceRelativeVelocity));

            previousSourcePosition = sourcePosition;
            previousListenerPosition = listenerPosition;

            return ((SpeedOfSound + listenerRelativeVelocity) / (SpeedOfSound + sourceRelativeVelocity));
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
                double pan = Lerp(tones[i].prevPan, tones[i].pan, fraction);
                pan+=stereoPanLerp;
                double volume = Lerp(tones[i].prevVolume, tones[i].volume, fraction);
                double frequency = Lerp(tones[i].prevFrequency, tones[i].frequency,fraction);
                frequency = frequency * Lerp(1,dopplerLerp,tones[i].advanced.doppler);
                tones[i].counter += t * (tones[i].advanced.useNoise?frequency*0.0001:frequency) * Mathf.PI * 2;

                if(!tones[i].advanced.useNoise)
                    tones[i].counter = tones[i].counter % (Mathf.PI * 2);
                else
                    tones[i].counter = tones[i].counter % 1e7;
                if(tones[i].advanced.useNoise){
                    double dn = noise.Noise(System.Math.Sin(tones[i].counter) * 10000,System.Math.Cos(tones[i].counter) * 10000, tones[i].counter);
                    double dn2 = noise.Noise(System.Math.Sin(tones[i].counter) * 100,System.Math.Cos(tones[i].counter) * 100, tones[i].counter);
                    double dn3 = noise.Noise(System.Math.Sin(tones[i].counter) * 1000,System.Math.Cos(tones[i].counter) * 1000, tones[i].counter);
                    dn += dn2 + dn3;
                    if (double.IsNaN(dn) || double.IsInfinity(dn)){
                        dn = 0;
                    }
                    tones[i].output = Lerp(tones[i].output, dn, 0.5) * volume;
                }
                else if(tones[i].advanced.useCustomCurve)
                    tones[i].output = volume * tones[i].advanced.curve.Evaluate((float)(tones[i].counter / (Math.PI * 2.0)));
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