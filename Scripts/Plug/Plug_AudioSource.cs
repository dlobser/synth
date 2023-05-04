using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Plug_AudioSource : MonoBehaviour
    {
        public AudioSource audio;
        public Oscillator volumeOscillator;
        public Oscillator pitchOscillator;
        public Oscillator panOscillator;
        public Trigger volumeTrigger;
        public Trigger pitchTrigger;
        public Trigger panTrigger;
        public bool forceAudioToPlay = true;

        [Header("Play Single Sample")]

        public Oscillator playOscillator;
        public Trigger playTrigger;
        //public bool playAtValue;
        public bool instantiateSource;
        List<AudioSource> sources;

        [Tooltip("The gameobject with an audio source to instantiate (not required)")]
        public GameObject audioInstance;

        // public bool ascending;
        public float playValue = 0;
        public bool crop;
        public float inTime;
        public float outTime;
        public float playSpeed;
        float prevPlaySpeed;
        float playTime;
        Coroutine player;
        float prevPlayOscillatorCounter;
        GameObject sampleParent;


        [Tooltip("Choose audio clips from audio sources in children")]

        public GameObject clipParent;
        public bool randomizeClips;
        int whichAudioClip;

        public Conductor conductor;
        public bool debug;



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
                Conductor[] conductors = FindObjectsOfType<Conductor>();
                Transform parentTransform = this.transform;
                while (parentTransform != null) // continue as long as there are parents
                {
                    for (int i = 0; i < conductors.Length; i++)
                    {
                        if (parentTransform.gameObject == conductors[i].gameObject)
                        {
                            conductor = conductors[i];
                            Debug.Log(this.gameObject.name + " Found Conductor: " + conductors[i].gameObject.name);
                            break;
                        }
                    }
                    parentTransform = parentTransform.parent; // move to the next parent
                }
            }
        }

        // IEnumerator PlayOscillate(){
        //     while(true){
        //         if(Time.time> playTime){

        //             if (clipParent != null)
        //             {
        //                 audio.clip = clipParent.transform.GetChild(Random.Range(0, clipParent.transform.childCount)).GetComponent<AudioSource>().clip;
        //             }
        //             if(!instantiateSource){

        //                 if (volumeOscillator)
        //                     audio.volume = volumeOscillator.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
        //                 else if(conductor!=null)
        //                     audio.volume = conductor.masterVolume;
        //                 if (pitchOscillator)
        //                     audio.pitch = pitchOscillator.GetValue();
        //                 if (panOscillator)
        //                     audio.panStereo = panOscillator.GetValue();

        //                 if(crop){
        //                     audio.time = inTime;
        //                 }
        //                 audio.Stop();
        //                 if(playSpeed!=prevPlaySpeed)
        //                     playTime = Mathf.Ceil(playTime);
        //                 audio.PlayScheduled(playTime + playSpeed);
        //                 print(playTime+playSpeed);
        //                 playTime += playSpeed;
        //             }
        //         }
        //         yield return null;
        //         prevPlaySpeed = playSpeed;
        //     }
        //     // else{
        //     //     GameObject g;
        //     //     if(audioInstance!=null)
        //     //         g = Instantiate(audioInstance);
        //     //     else{
        //     //         g = new GameObject(this.name + "_Note");
        //     //         g.AddComponent<AudioSource>();
        //     //     }

        //     //     AudioSource a = g.GetComponent<AudioSource>();
        //     //     a.clip = audio.clip;
        //     //     if (volumeOscillator)
        //     //         a.volume = volumeOscillator.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
        //     //     else if(conductor!=null)
        //     //         a.volume = conductor.masterVolume;
        //     //     if (pitchOscillator)
        //     //         a.pitch = pitchOscillator.GetValue();
        //     //     if (panOscillator)
        //     //         a.panStereo = panOscillator.GetValue();
        //     //     if(crop){
        //     //         a.time = inTime;
        //     //     }

        //     //     a.Play();
        //     //     sources.Add(a);
        //     //     if(sampleParent==null){
        //     //         sampleParent = new GameObject("SampleParent");
        //     //         sampleParent.transform.parent = this.transform;
        //     //     }
        //     //     g.transform.parent = sampleParent.transform;
        //     // }

        // }

        // Update is called once per frame
        void Update()
        {

            // if(playOscillator){
            //     if(player==null){
            //         player = StartCoroutine(PlayOscillate());
            //     }
            //     playSpeed = playOscillator.GetValue();
            // }
            // else if(player!=null){
            //     StopCoroutine(player);
            //     player = null;
            // }
            if (volumeOscillator != null)
            {
                audio.volume = volumeOscillator.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
                if (volumeTrigger != null)
                {
                    audio.volume *= volumeTrigger.GetValue();
                    if (debug)
                    {
                        print(volumeTrigger.GetValue());
                        print(audio.volume);
                    }
                }
            }
            else if (volumeTrigger != null)
                audio.volume = volumeTrigger.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
            else if (conductor != null)
                audio.volume = conductor.masterVolume;


            if (pitchOscillator != null)
            {
                audio.pitch = pitchOscillator.GetValue();
                if (pitchTrigger != null)
                    audio.pitch *= pitchTrigger.GetValue();
            }
            else if (pitchTrigger != null)
                audio.pitch = pitchTrigger.GetValue();

            if (panOscillator != null)
            {
                audio.panStereo = panOscillator.GetValue();
                if (panTrigger != null)
                    audio.panStereo *= panTrigger.GetValue();
            }
            else if (panTrigger != null)
                audio.panStereo = panTrigger.GetValue();



            // if (volumeOscillator)
            //     audio.volume = volumeOscillator.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
            // else if (conductor != null)
            //     audio.volume = conductor.masterVolume;
            // if (pitchOscillator)
            //     audio.pitch = pitchOscillator.GetValue();
            // if (panOscillator)
            //     audio.panStereo = panOscillator.GetValue();

            if (playOscillator || playTrigger)
            {
                float p = 0;
                if (playOscillator)
                    p = playOscillator.GetValue();
                if (playTrigger)
                    p = playTrigger.GetValue();

                if (p > playValue && prevPlayOscillatorCounter < playValue)
                {
                    if (clipParent != null)
                    {
                        audio.clip = clipParent.transform.GetChild(Random.Range(0, clipParent.transform.childCount)).GetComponent<AudioSource>().clip;
                        if (!randomizeClips)
                        {
                            audio.clip = clipParent.transform.GetChild(whichAudioClip).GetComponent<AudioSource>().clip;
                            whichAudioClip++;
                            if (whichAudioClip >= clipParent.transform.childCount)
                            {
                                whichAudioClip = 0;
                            }
                        }
                    }
                    if (!instantiateSource)
                    {

                        if (volumeOscillator)
                            audio.volume = volumeOscillator.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
                        else if (conductor != null)
                            audio.volume = conductor.masterVolume;
                        if (pitchOscillator)
                            audio.pitch = pitchOscillator.GetValue();
                        if (panOscillator)
                            audio.panStereo = panOscillator.GetValue();

                        if (crop)
                        {
                            audio.time = inTime;
                        }
                        audio.Stop();
                        audio.Play();
                    }
                    else
                    {
                        GameObject g;
                        if (audioInstance != null)
                            g = Instantiate(audioInstance);
                        else
                        {
                            g = new GameObject(this.name + "_Note");
                            g.AddComponent<AudioSource>();
                        }

                        AudioSource a = g.GetComponent<AudioSource>();
                        a.clip = audio.clip;
                        if (volumeOscillator)
                            a.volume = volumeOscillator.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
                        else if (conductor != null)
                            a.volume = conductor.masterVolume;
                        if (pitchOscillator)
                            a.pitch = pitchOscillator.GetValue();
                        if (panOscillator)
                            a.panStereo = panOscillator.GetValue();
                        if (crop)
                        {
                            a.time = inTime;
                        }

                        a.Play();
                        sources.Add(a);
                        if (sampleParent == null)
                        {
                            sampleParent = new GameObject("SampleParent");
                            sampleParent.transform.parent = this.transform;
                        }
                        g.transform.parent = sampleParent.transform;
                    }
                }
                prevPlayOscillatorCounter = p;
            }
            else if (!audio.isPlaying && forceAudioToPlay)
            {
                audio.Play();
            }
            if (crop && audio.isPlaying && audio.time > outTime)
            {
                audio.Stop();
            }
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