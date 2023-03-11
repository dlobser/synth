using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_AudioInstantiator : MonoBehaviour
    {
        // public AudioSource audio;
        public AudioSource[] audioSources;
        float[] initialAudioVolumes;
        int whichAudio;

        public Oscillator oscillator;
        public Trigger trigger;

        public float frequency;
        public float offset;

        public bool instantiateSource;
        List<AudioSource> sources;

        [Tooltip("The gameobject with an audio source to instantiate (not required)")]
        public GameObject audioInstance;
        // public bool ascending;
        // public float playValue = 0;
        public bool crop;
        public float inTime;
        public float outTime;
        public float playSpeed;
        float prevPlaySpeed;
        float playTime;
        GameObject sampleParent;
        Coroutine player;

        [Tooltip("Choose audio clips from audio sources in children")]

        public GameObject clipParent;

        public Conductor conductor;
        public bool debug;
        bool frequencyChanged = false;
        public bool resetAllCounters = false;

        void Start()
        {
            if(sampleParent!=null){
                Destroy(sampleParent);
            }
            if(this.transform.Find("SampleParent")!=null){
                Destroy(this.transform.Find("SampleParent").gameObject);
            }
            sources = new List<AudioSource>();
            if(conductor==null){
                conductor = FindObjectOfType<Conductor>();
            }
            playTime = (float) AudioSettings.dspTime;
            initialAudioVolumes = new float[audioSources.Length];
            for (int i = 0; i < audioSources.Length; i++)
            {
                initialAudioVolumes[i] = audioSources[i].volume;
            }
        }

        void ResetAllCounters()
        {
            Plug_AudioInstantiator[] plugs = FindObjectsOfType<Plug_AudioInstantiator>();
            ResetCounter();
            foreach (Plug_AudioInstantiator o in plugs)
            {
                o.ResetCounter();
            }
        }

        public void ResetCounter(){
            frequencyChanged = true;
        }

        void Update()
        {
            float conductorVolume = 1;
            float conductorSpeed = 1;

            if(conductor!=null){
                conductorVolume = conductor.masterVolume;
                conductorSpeed = conductor.masterSpeed;
            }
            if(conductorSpeed==0){
                conductorSpeed = 1;
            }

            if(resetAllCounters){
                resetAllCounters = false;
                ResetAllCounters();
            }

            if(AudioSettings.dspTime > playTime){


                if (clipParent != null)
                {
                    GetComponent<AudioSource>().clip = clipParent.transform.GetChild(Random.Range(0, clipParent.transform.childCount)).GetComponent<AudioSource>().clip;
                }
                if(!instantiateSource){

                    if(crop){
                        GetComponent<AudioSource>().time = inTime;
                    }
                    // audio.Stop();
                    if(frequencyChanged){
                        print("Adjust: " + playTime + " , " + Mathf.Ceil((float)playTime));
                        playTime = Mathf.Ceil((float)playTime) + offset;
                        frequencyChanged = false;
                        
                    }
                    if(Synth_Util.GetOscTrigValue(oscillator,trigger)>=0){
                        if(audioSources.Length>0){
                            audioSources[whichAudio].PlayScheduled(playTime + (frequency/conductorSpeed));
                            audioSources[whichAudio].volume = initialAudioVolumes[whichAudio]*conductorVolume;
                            whichAudio++;
                            if( whichAudio >= audioSources.Length )
                                whichAudio = 0;
                        }
                        else
                            GetComponent<AudioSource>().PlayScheduled(playTime + (frequency/conductorSpeed));
                    }
                    // print(AudioSettings.dspTime + " , " + playTime + " , " + (playTime+(frequency/conductorSpeed)));
                    playTime += (frequency/conductorSpeed);

                    
                }
                else{
                    float initialVolume = 1;
                    GameObject g;
                     if(audioInstance!=null)
                        g = Instantiate(audioInstance);
                    else if(audioSources.Length>0){
                        g = Instantiate(audioSources[whichAudio].gameObject);
                        initialVolume = initialAudioVolumes[whichAudio];
                        whichAudio++;
                        if( whichAudio >= audioSources.Length )
                            whichAudio = 0;
                    }
                    else{
                        g = new GameObject(this.name + "_Note");
                        g.AddComponent<AudioSource>();
                    }
                    
                    AudioSource a = g.GetComponent<AudioSource>();
                    // a.clip = GetComponent<AudioSource>().clip;
                    
                    if(crop){
                        a.time = inTime;
                    }
                    
                    if(frequencyChanged){
                        print("Adjust: " + playTime + " , " + Mathf.Ceil((float)playTime));
                        playTime = Mathf.Ceil((float)playTime) + offset;
                        frequencyChanged = false;
                       
                    }

                    if(Synth_Util.GetOscTrigValue(oscillator,trigger)>=0){
                        if(Synth_Util.GetOscTrigValue(oscillator,trigger)>=0){
                            a.PlayScheduled(playTime + (frequency/conductorSpeed));
                            a.volume = initialVolume*conductorVolume;
                        }
                    }

                    playTime += (frequency/conductorSpeed);
                    // a.PlayScheduled(playTime + frequency);

                    sources.Add(a);
                    if(sampleParent==null){
                        sampleParent = new GameObject("SampleParent");
                        sampleParent.transform.parent = this.transform;
                    }
                    g.transform.parent = sampleParent.transform;
                }
                
            }




            // if(playOscillator){
                // if(player==null){
                //     player = StartCoroutine(PlayOscillate());
                // }
                if(prevPlaySpeed!=frequency)
                    frequencyChanged = true;
                
                prevPlaySpeed = frequency;
                // print(player==null);
            //     playSpeed = playOscillator.GetValue();
            // }
            // else if(player!=null){
            //     StopCoroutine(player);
            //     player = null;
            // }
           
            // if (playOscillator || playTrigger)
            // {
            //     float p = 0;
            //     if (playOscillator)
            //         p = playOscillator.GetValue();
            //     if (playTrigger)
            //         p = playTrigger.GetValue();

            //     if (p > playValue && prevPlayOscillatorCounter < playValue)
            //     {
            //         if (clipParent != null)
            //         {
            //             audio.clip = clipParent.transform.GetChild(Random.Range(0, clipParent.transform.childCount)).GetComponent<AudioSource>().clip;
            //         }
            //         if(!instantiateSource){

            //             if (volumeOscillator)
            //                 audio.volume = volumeOscillator.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
            //             else if(conductor!=null)
            //                 audio.volume = conductor.masterVolume;
            //             if (pitchOscillator)
            //                 audio.pitch = pitchOscillator.GetValue();
            //             if (panOscillator)
            //                 audio.panStereo = panOscillator.GetValue();

            //             if(crop){
            //                 audio.time = inTime;
            //             }
            //             audio.Stop();
            //             audio.Play();
            //         }
            //         else{
            //             GameObject g;
            //             if(audioInstance!=null)
            //                 g = Instantiate(audioInstance);
            //             else{
            //                 g = new GameObject(this.name + "_Note");
            //                 g.AddComponent<AudioSource>();
            //             }
                        
            //             AudioSource a = g.GetComponent<AudioSource>();
            //             a.clip = audio.clip;
            //             if (volumeOscillator)
            //                 a.volume = volumeOscillator.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
            //             else if(conductor!=null)
            //                 a.volume = conductor.masterVolume;
            //             if (pitchOscillator)
            //                 a.pitch = pitchOscillator.GetValue();
            //             if (panOscillator)
            //                 a.panStereo = panOscillator.GetValue();
            //             if(crop){
            //                 a.time = inTime;
            //             }
                        
            //             a.Play();
            //             sources.Add(a);
            //             if(sampleParent==null){
            //                 sampleParent = new GameObject("SampleParent");
            //                 sampleParent.transform.parent = this.transform;
            //             }
            //             g.transform.parent = sampleParent.transform;
            //         }
            //     }
            //     prevPlayOscillatorCounter = p;
            // }
            // else if(!audio.isPlaying && forceAudioToPlay){
            //     audio.Play();
            // }
            // if(crop&&audio.isPlaying&&audio.time>outTime){
            //     audio.Stop();
            // }
            for (int i = 0; i < sources.Count; i++)
            {
                if(crop&&sources[i].isPlaying&&sources[i].time>outTime){
                    sources[i].Stop();
                }
                if(!sources[i].isPlaying){
                    Destroy(sources[i].gameObject);
                    sources.RemoveAt(i);
                }
            }
        }
    }
}