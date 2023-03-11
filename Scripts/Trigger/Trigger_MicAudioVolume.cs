using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_MicAudioVolume : Trigger {
    
       public static float MicLoudness;

        private string _device;
        public int whichAudioDevice;

        public bool valueIsPitch = false;

        AudioSource audioSource;

        public Plug_AudioSynth plug; 
        float plugLerp = 0;

        //mic initialization
        void InitMic(){
            int w = 0;
            foreach(string s in Microphone.devices){
                print("Mic: " + w + " - " + s);
                w++;
            }
            if(_device == null) _device = Microphone.devices[whichAudioDevice];
            _clipRecord = Microphone.Start(_device, true, 3, 44100);
        }

        void StopMicrophone()
        {
            Microphone.End(_device);
        }

        AudioClip _clipRecord = null;//new AudioClip();
        int _sampleWindow = 128;

        //get data from microphone into audioclip
        float  LevelMax()
        {
            float levelMax = 0;
            float[] waveData = new float[_sampleWindow];
            int micPosition = Microphone.GetPosition(_device)-(_sampleWindow+1); // null means the first microphone
            if (micPosition < 0) return 0;
            _clipRecord.GetData(waveData, micPosition);
            // Getting a peak on the last 128 samples
            for (int i = 0; i < _sampleWindow; i++) {
                float wavePeak = waveData[i] * waveData[i];
                if (levelMax < wavePeak) {
                    levelMax = wavePeak;
                }
            }
            return levelMax;
        }


        void Update()
        {
            // levelMax equals to the highest normalized value power 2, a small number because < 1
            // pass the value to a static var so we can access it from anywhere
            if(!valueIsPitch)
                value = LevelMax ();
            else{
                value = AnalyzeSound();
                plugLerp = Mathf.Lerp(plugLerp,value,.1f);
                if(plug!=null){
                    plug.tones[0].frequency = value;
                }
            }
            

            // value = MicLoudness;
        }

        bool _isInitialized;
        // start mic when scene starts
        void OnEnable()
        {
            InitMic();
            _isInitialized=true;
        }

        //stop mic when loading a new level or quit application
        void OnDisable()
        {
            StopMicrophone();
        }

        void OnDestroy()
        {
            StopMicrophone();
        }


        // make sure the mic gets started & stopped when application gets focused
        void OnApplicationFocus(bool focus) {
            if (focus)
            {
                //Debug.Log("Focus");

                if(!_isInitialized){
                    //Debug.Log("Init Mic");
                    InitMic();
                    _isInitialized=true;
                }
            }      
            if (!focus)
            {
                //Debug.Log("Pause");
                StopMicrophone();
                //Debug.Log("Stop Mic");
                _isInitialized=false;

            }
        }

        float AnalyzeSound()
        {
            LevelMax();
            // print(Microphone.IsRecording(_device));
            if(!Microphone.IsRecording(_device)){
                _clipRecord = Microphone.Start(_device, true, 3, 44100);

            }
            // float levelMax = 0;
            // float[] waveData = new float[_sampleWindow];
            // int micPosition = Microphone.GetPosition(_device)-(_sampleWindow+1); // null means the first microphone
            // if (micPosition < 0) return 0;
            // _clipRecord.GetData(waveData, micPosition);

            if(audioSource==null)
                audioSource = this.gameObject.AddComponent<AudioSource>();
            
            // audioSource.loop = true;
            if(!audioSource.isPlaying){
                audioSource.clip = _clipRecord;
                audioSource.Play();
            }

            int QSamples = 1024;
            float[] _samples;
            float[] _spectrum;

            _samples = new float[QSamples];
            _spectrum = new float[QSamples];
            // _clipRecord.GetData(_samples, micPosition);

            audioSource.GetOutputData(_samples, 0); // fill array with samples
            int i;
            float sum = 0;
            for (i = 0; i < QSamples; i++)
            {
                sum += _samples[i] * _samples[i]; // sum squared samples
            }
            float rmsVal = Mathf.Sqrt(sum / QSamples); // rms = square root of average
            float RefValue = .1f;
            float dbVal = 20 * Mathf.Log10(rmsVal / RefValue); // calculate dB
            if (dbVal < -160) dbVal = -160; // clamp it to -160dB min
                                            // get sound spectrum

           

            audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
            float maxV = 0;
            var maxN = 0;
            float Threshold = 0.02f;
            for (i = 0; i < QSamples; i++)
            { // find max 
                if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                    continue;

                maxV = _spectrum[i];
                maxN = i; // maxN is the index of max
            }
            float freqN = maxN; // pass the index to a float variable
            if (maxN > 0 && maxN < QSamples - 1)
            { // interpolate index using neighbours
                var dL = _spectrum[maxN - 1] / _spectrum[maxN];
                var dR = _spectrum[maxN + 1] / _spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            float _fSample = AudioSettings.outputSampleRate;
            return freqN * (_fSample / 2) / QSamples; // convert index to frequency
        }
    }
 }


// public class MicInput : MonoBehaviour {

//         public static float MicLoudness;

//         private string _device;

//         //mic initialization
//         void InitMic(){
//             if(_device == null) _device = Microphone.devices[0];
//             _clipRecord = Microphone.Start(_device, true, 999, 44100);
//         }

//         void StopMicrophone()
//         {
//             Microphone.End(_device);
//         }


//         AudioClip _clipRecord = new AudioClip();
//         int _sampleWindow = 128;

//         //get data from microphone into audioclip
//         float  LevelMax()
//         {
//             float levelMax = 0;
//             float[] waveData = new float[_sampleWindow];
//             int micPosition = Microphone.GetPosition(null)-(_sampleWindow+1); // null means the first microphone
//             if (micPosition < 0) return 0;
//             _clipRecord.GetData(waveData, micPosition);
//             // Getting a peak on the last 128 samples
//             for (int i = 0; i < _sampleWindow; i++) {
//                 float wavePeak = waveData[i] * waveData[i];
//                 if (levelMax < wavePeak) {
//                     levelMax = wavePeak;
//                 }
//             }
//             return levelMax;
//         }



//         void Update()
//         {
//             // levelMax equals to the highest normalized value power 2, a small number because < 1
//             // pass the value to a static var so we can access it from anywhere
//             MicLoudness = LevelMax ();
//         }

//         bool _isInitialized;
//         // start mic when scene starts
//         void OnEnable()
//         {
//             InitMic();
//             _isInitialized=true;
//         }

//         //stop mic when loading a new level or quit application
//         void OnDisable()
//         {
//             StopMicrophone();
//         }

//         void OnDestroy()
//         {
//             StopMicrophone();
//         }


//         // make sure the mic gets started & stopped when application gets focused
//         void OnApplicationFocus(bool focus) {
//             if (focus)
//             {
//                 //Debug.Log("Focus");

//                 if(!_isInitialized){
//                     //Debug.Log("Init Mic");
//                     InitMic();
//                     _isInitialized=true;
//                 }
//             }      
//             if (!focus)
//             {
//                 //Debug.Log("Pause");
//                 StopMicrophone();
//                 //Debug.Log("Stop Mic");
//                 _isInitialized=false;

//             }
//         }
//     }
