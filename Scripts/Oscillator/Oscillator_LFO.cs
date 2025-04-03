using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// using Unity.Mathematics;

namespace ON.synth
{
    [System.Serializable]
    public class Oscillator_LFO : Oscillator
    {
        public string info;
        public AnimationCurve displayCurve;
        public float displayCurveRange = 1;
        [HideInInspector]
        public bool resetAllCounters;

        [Header("Values")]
        public float trough = -1;
        public float crest = 1;
        public float clampLow = -1000;
        public float clampHigh = 1000;
        public float offset;
        public float timeOffset;
        public float multiply = 1;
        public float speed = 1;
        public float counterPower = 1;
        public float sinPower = 1;
        public float quantize = 0;

        float counter;
        float prevCounter;

        [Header("Options")]
        public bool useNoise;
        public bool dontLoopCustomCurve;
        public bool useCustom;
        public AnimationCurve curve;

        public float lazyValue = 0;
        public float lazyLerpSpeed = 1000;

        public Oscillators oscillators;
        public Triggers triggers;

        [Header("Display")]
        public LineRenderer line;
        public bool showLine;
        public int lineLength = 100;

        public Conductor conductor;
        public bool debug;

        float sampleRate;

        PerlinNoise pNoise;

        void Start()
        {
            if(float.IsNaN(lazyValue)){
                lazyValue = 0;
            }
            if(curve==null){
                curve = new AnimationCurve();
                curve.AddKey(new Keyframe(0,0));
            }
            counter = 0;

            pNoise = new PerlinNoise(0);

            if (conductor == null)
            {
                Conductor[] conductors = FindObjectsOfType<Conductor>();
                Transform parentTransform = this.transform;
                while (parentTransform != null) // continue as long as there are parents
                {
                    for (int i = 0; i < conductors.Length; i++)
                    {
                        if (parentTransform.gameObject == conductors[i].gameObject)
                        {
                            conductor = conductors[i];
                            // target GameObject is in the parent hierarchy
                            Debug.Log(this.gameObject.name + " Found Conductor: " + conductors[i].gameObject.name);
                            break;
                        }
                    }
                    parentTransform = parentTransform.parent; // move to the next parent
                }
            }

            sampleRate = AudioSettings.outputSampleRate;

            for (int i = 0; i < 60; i++)
            {
                float v = ((float)i / 60f);
                if(displayCurve!=null)
                    displayCurve.AddKey(new Keyframe(v, GetValue(v * displayCurveRange)));
            }
        }

        void OnDisable()
        {
            this.counter = 0;
        }

        void GetValues()
        {   
            if(oscillators!=null){
                multiply = ON.synth.Synth_Util.GetOscTrigValue(oscillators.multiplyOscillate, triggers.multiplyTrigger, multiply);
                speed = ON.synth.Synth_Util.GetOscTrigValue(oscillators.speedOscillate, triggers.speedTrigger, speed);
                offset = ON.synth.Synth_Util.GetOscTrigValue(oscillators.offsetOscillate, triggers.offsetTrigger, offset);
                trough = ON.synth.Synth_Util.GetOscTrigValue(oscillators.troughOscillate, triggers.troughTrigger, trough);
                crest = ON.synth.Synth_Util.GetOscTrigValue(oscillators.crestOscillate, triggers.crestTrigger, crest);
                clampLow = ON.synth.Synth_Util.GetOscTrigValue(oscillators.clampLowOscillate, triggers.clampLowTrigger, clampLow);
                clampHigh = ON.synth.Synth_Util.GetOscTrigValue(oscillators.clampHighOscillate, triggers.clampHighTrigger, clampHigh);
                timeOffset = ON.synth.Synth_Util.GetOscTrigValue(oscillators.timeOffsetOscillate, triggers.timeOffsetTrigger, timeOffset);
                quantize = ON.synth.Synth_Util.GetOscTrigValue(oscillators.quantizeOscillate, triggers.quantizeTrigger, quantize);
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            float masterSpeed = conductor != null ? conductor.masterSpeed : 1;
            this.counter += (((float)data.Length / (float)channels) / (float)sampleRate) * (speed * masterSpeed != 0 ? (1 / (masterSpeed * speed)) : 0);
            if (!useNoise && !dontLoopCustomCurve)
                this.counter = this.counter % 1;
        }

        void Update()
        {
            //counting happens in onaudiofilteread for more accuracy if an audiosource is connected.
            //The audiosource doesn't need to be playing audio
            if (GetComponent<AudioSource>() == null)
            {
                float masterSpeed = conductor != null ? conductor.masterSpeed : 1;
                this.counter += Time.deltaTime * (speed * masterSpeed != 0 ? (1 / (masterSpeed * speed)) : 0);
                if (!useNoise && !dontLoopCustomCurve)
                    this.counter = this.counter % 1;
            }

            GetValues();

            prevCounter = counter;

            if (resetAllCounters)
            {
                ResetAllCounters();
                resetAllCounters = false;
            }
            if (line == null && showLine)
            {
                line = this.AddComponent<LineRenderer>();
                line.widthMultiplier = .1f;
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.useWorldSpace = false;
                line.material.color = Color.white;
            }
            if (line != null && showLine)
            {
                line.enabled = true;
                line.positionCount = Mathf.Max(1, lineLength);
                Vector3[] l = new Vector3[lineLength];

                for (int i = 0; i < l.Length; i++)
                {
                    float c = ((float)i / 60f);// * (speed != 0 ? (1 / speed) : 0);
                    l[i] = new Vector3((float)i / 60, GetValue(c), 0);
                }
                line.SetPositions(l);
            }
            if (line != null && !showLine)
            {
                line.enabled = false;
            }

            if (displayCurve == null)
            {
                displayCurve = new AnimationCurve();
            }
            if (displayCurve.keys == null)
            {
                for (int i = 0; i < 60; i++)
                {
                    float v = ((float)i / 60f);
                    displayCurve.AddKey(new Keyframe(v, GetValue(v * displayCurveRange)));
                }
            }
            if (displayCurve.keys.Length < 60)
            {
                for (int i = 0; i < 60; i++)
                {
                    float v = ((float)i / 60f);
                    displayCurve.AddKey(new Keyframe(v, GetValue(v * displayCurveRange)));
                }
            }

            for (int i = 0; i < 60; i++)
            {
                float v = ((float)i / 60f);
                displayCurve.MoveKey(i, new Keyframe(v, GetValue(v * displayCurveRange)));
            }

            info = "Counter: " + counter + ", " + "Value: " + GetValue();
        }

        public void ResetAllCounters()
        {
            Oscillator[] oscillators = FindObjectsOfType<Oscillator>();
            ResetCounter();
            foreach (Oscillator o in oscillators)
            {
                o.ResetCounter();
            }
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource a in audioSources)
            {
                a.Stop();
                a.Play();
            }
        }

        public override void ResetCounter()
        {
            counter = 0;
        }

        float map(float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        public bool Ascending()
        {
            float value = GetValue();
            float prevValue = GetValue(prevCounter);
            return value > prevValue;
        }

        public override float GetValue()
        {
            float s;

            float qcounter = counter;

            if (quantize > 0)
            {
                qcounter *= quantize;
                qcounter = Mathf.Floor(qcounter);
                qcounter /= quantize;
            }

            if (useNoise && pNoise!=null)
                s = ((float)pNoise.Noise(Mathf.Pow(qcounter, counterPower) + (timeOffset), 0,0)*.95f)+.5f;// Mathf.PerlinNoise(Mathf.Pow(qcounter, counterPower) + (timeOffset), 0);
            else if (useCustom && curve!=null)
                s = curve.Evaluate(Mathf.Pow(qcounter, counterPower) + (timeOffset));
            else
                s = Mathf.Cos(Mathf.Pow(qcounter, counterPower) * Mathf.PI * 2 + (timeOffset % 1) * Mathf.PI * 2);

            float lBound = -1f;

            if (useNoise || useCustom)
                lBound = 0;

            float output = Mathf.Clamp(map(s, lBound, 1, trough, crest) * multiply + offset, clampLow, clampHigh);
            if (debug)
            {
                print("Output: " + output);
            }
            output = Mathf.Pow(output, output > 0 ? sinPower : 1);

            lazyValue = Mathf.Lerp(lazyValue, output, lazyLerpSpeed * Time.deltaTime);
            if(float.IsNaN(lazyValue)){
                lazyValue = 0;
            }
            return lazyValue;
        }

        public override float GetValue(float c)
        {

            float _multiply = multiply;
            float _speed = speed;
            float _offset = offset;
            float _trough = trough;
            float _crest = crest;
            float _clampLow = clampLow;
            float _clampHigh = clampHigh;
            float _timeOffset = timeOffset;

            float counter = c * (_speed != 0 ? (1 / _speed) : 0);
            if (!useNoise)
                counter = counter % 1;

            float t = c;
            
            if(oscillators!=null){
                multiply = ON.synth.Synth_Util.GetOscTrigValue(oscillators.multiplyOscillate, triggers.multiplyTrigger, multiply);
                speed = ON.synth.Synth_Util.GetOscTrigValue(oscillators.speedOscillate, triggers.speedTrigger, speed);
                offset = ON.synth.Synth_Util.GetOscTrigValue(oscillators.offsetOscillate, triggers.offsetTrigger, offset);
                trough = ON.synth.Synth_Util.GetOscTrigValue(oscillators.troughOscillate, triggers.troughTrigger, trough);
                crest = ON.synth.Synth_Util.GetOscTrigValue(oscillators.crestOscillate, triggers.crestTrigger, crest);
                clampLow = ON.synth.Synth_Util.GetOscTrigValue(oscillators.clampLowOscillate, triggers.clampLowTrigger, clampLow);
                clampHigh = ON.synth.Synth_Util.GetOscTrigValue(oscillators.clampHighOscillate, triggers.clampHighTrigger, clampHigh);
                timeOffset = ON.synth.Synth_Util.GetOscTrigValue(oscillators.timeOffsetOscillate, triggers.timeOffsetTrigger, timeOffset);
                quantize = ON.synth.Synth_Util.GetOscTrigValue(oscillators.quantizeOscillate, triggers.quantizeTrigger, quantize);
            }

            float s;

            if (quantize > 0)
            {
                counter *= quantize;
                counter = Mathf.Floor(counter);
                counter /= quantize;
            }

            if (useNoise)
                s = ((float)pNoise.Noise(Mathf.Pow(counter, counterPower) + (timeOffset), 0,0)*.95f)+.5f;// Mathf.PerlinNoise(Mathf.Pow(qcounter, counterPower) + (timeOffset), 0);
//Mathf.PerlinNoise(Mathf.Pow(counter, counterPower) + (timeOffset), 0);
            else if (useCustom && curve!=null)
                s = curve.Evaluate(Mathf.Pow(counter, counterPower) + (timeOffset));
            else
                s = Mathf.Cos(Mathf.Pow(counter, counterPower) * Mathf.PI * 2 + (timeOffset % 1) * Mathf.PI * 2);

            float lBound = -1f;
            if (useNoise)// || useCustom)
                lBound = 0;
            float output = Mathf.Clamp(map(s, lBound, 1, _trough, _crest) * _multiply + _offset, _clampLow, _clampHigh);
            output = Mathf.Pow(output, output > 0 ? sinPower : 1);
            return output;
        }
    }

}