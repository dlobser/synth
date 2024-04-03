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
        public bool playRandom;
        int chooseAudio;

        Coroutine player;

        [Tooltip("Choose audio clips from audio sources in children")]

        public GameObject clipParent;

        public Conductor conductor;
        public bool debug;
        bool frequencyChanged = false;
        public bool resetAllCounters = false;

        void Start()
        {
            if (sampleParent != null)
            {
                Destroy(sampleParent);
            }
            if (this.transform.Find("SampleParent") != null)
            {
                Destroy(this.transform.Find("SampleParent").gameObject);
            }
            sources = new List<AudioSource>();
            if (conductor == null)
            {
                conductor = FindObjectOfType<Conductor>();
            }
            playTime = (float)AudioSettings.dspTime;
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

        public void ResetCounter()
        {
            frequencyChanged = true;
        }

        void Update()
        {
            float conductorVolume = 1;
            float conductorSpeed = 1;

            if (conductor != null)
            {
                conductorVolume = conductor.masterVolume;
                conductorSpeed = conductor.masterSpeed;
            }
            if (conductorSpeed == 0)
            {
                conductorSpeed = 1;
            }

            if (resetAllCounters)
            {
                resetAllCounters = false;
                ResetAllCounters();
            }

            if (AudioSettings.dspTime > playTime)
            {



                if (!instantiateSource)
                {
                    if (clipParent != null)
                    {
                        int index = !playRandom ? chooseAudio : Random.Range(0, clipParent.transform.childCount);
                        GetComponent<AudioSource>().clip = clipParent.transform.GetChild(index).GetComponent<AudioSource>().clip;
                        chooseAudio++;
                        if (chooseAudio > clipParent.transform.childCount - 1)
                        {
                            chooseAudio = 0;
                        }
                    }
                    if (crop)
                    {
                        GetComponent<AudioSource>().time = inTime;
                    }
                    // audio.Stop();
                    // if (frequencyChanged)
                    // {
                    //     print("Adjust: " + playTime + " , " + Mathf.Ceil((float)playTime));
                    //     playTime = Mathf.Ceil((float)playTime) + offset;
                    //     frequencyChanged = false;

                    // }
                    if (frequencyChanged)
                    {
                        float currentTime = (float)AudioSettings.dspTime;
                        playTime = currentTime - ((currentTime - playTime) % (frequency));// + frequency;// (frequency / conductorSpeed) - ((currentTime - playTime) % (frequency / conductorSpeed));
                        frequencyChanged = false;
                    }
                    if (Synth_Util.GetOscTrigValue(oscillator, trigger) >= 0)
                    {
                        if (audioSources.Length > 0)
                        {
                            audioSources[whichAudio].PlayScheduled(playTime + (frequency / conductorSpeed));
                            audioSources[whichAudio].volume = initialAudioVolumes[whichAudio] * conductorVolume;
                            whichAudio++;
                            if (whichAudio >= audioSources.Length)
                                whichAudio = 0;
                        }
                        else
                            GetComponent<AudioSource>().PlayScheduled(playTime + (frequency / conductorSpeed));
                    }
                    // print(AudioSettings.dspTime + " , " + playTime + " , " + (playTime+(frequency/conductorSpeed)));
                    playTime += (frequency / conductorSpeed);


                }
                else
                {
                    float initialVolume = 1;
                    GameObject g;
                    if (audioInstance != null)
                        g = Instantiate(audioInstance);
                    else if (audioSources.Length > 0)
                    {
                        g = Instantiate(audioSources[whichAudio].gameObject);
                        initialVolume = initialAudioVolumes[whichAudio];
                        whichAudio++;
                        if (whichAudio >= audioSources.Length)
                            whichAudio = 0;
                    }
                    else
                    {
                        g = new GameObject(this.name + "_Note");
                        g.AddComponent<AudioSource>();
                    }


                    AudioSource a = g.GetComponent<AudioSource>();
                    if (a.clip == null)
                    {
                        if (clipParent != null)
                        {
                            int index = !playRandom ? chooseAudio : Random.Range(0, clipParent.transform.childCount);
                            if (clipParent.transform.GetChild(index).GetComponent<AudioSource>().clip != null)
                                a.clip = clipParent.transform.GetChild(index).GetComponent<AudioSource>().clip;
                            chooseAudio++;
                            if (chooseAudio > clipParent.transform.childCount - 1)
                            {
                                chooseAudio = 0;
                            }
                            print(chooseAudio);
                        }
                    }
                    // a.clip = GetComponent<AudioSource>().clip;

                    if (crop)
                    {
                        a.time = inTime;
                    }

                    if (frequencyChanged)
                    {
                        print("Adjust: " + playTime + " , " + Mathf.Ceil((float)playTime));
                        playTime = Mathf.Ceil((float)playTime) + offset;
                        frequencyChanged = false;

                    }


                    if (Synth_Util.GetOscTrigValue(oscillator, trigger) >= 0)
                    {
                        // if (Synth_Util.GetOscTrigValue(oscillator, trigger) >= 0)
                        // {
                        a.PlayScheduled(playTime + (frequency / conductorSpeed));
                        a.volume = initialVolume * conductorVolume;
                        // }
                    }

                    playTime += (frequency / conductorSpeed);
                    // a.PlayScheduled(playTime + frequency);

                    sources.Add(a);
                    if (sampleParent == null)
                    {
                        sampleParent = new GameObject("SampleParent");
                        sampleParent.transform.parent = this.transform;
                    }
                    g.transform.parent = sampleParent.transform;
                }

            }

            if (prevPlaySpeed != frequency)
                frequencyChanged = true;

            prevPlaySpeed = frequency;
            
            for (int i = 0; i < sources.Count; i++)
            {
                if (crop && sources[i].isPlaying && sources[i].time > outTime)
                {
                    sources[i].Stop();
                }
                if (!sources[i].isPlaying)
                {
                    Destroy(sources[i].gameObject);
                    sources.RemoveAt(i);
                }
            }
        }
    }
}