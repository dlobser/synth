using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AudioHelm
using AudioHelm;
#endif
using System;
using UnityEngine.Events;

namespace ON.synth
{

    public class Plug_Helm : MonoBehaviour
    {
        // public AudioSource audio;
#if AudioHelm
        public HelmController controller;
#endif
        public Oscillator volumeOscillator;
        public Oscillator pitchOscillator;
        public Oscillator panOscillator;
        // public bool forceAudioToPlay = true;
        float volume, pitch, panStereo;

        [Header("Play Single Sample")]

        public Oscillator playOscillator;
        public Trigger playTrigger;
        //public bool playAtValue;
        public bool instantiateSource;
        // List<AudioSource> sources;

        [Tooltip("The gameobject with an audio source to instantiate (not required)")]
        // public GameObject audioInstance;
        // public bool ascending;
        float playValue = 0;
        public bool crop;
        public float inTime;
        public float outTime;
        float prevPlayOscillatorCounter;
        GameObject sampleParent;

        public event System.Action NoteOn;
        public event System.Action NoteOff;

        [Tooltip("Choose audio clips from audio sources in children")]

        public GameObject clipParent;

        public Conductor conductor;

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
            // sources = new List<AudioSource>();
            if (conductor == null)
            {
                conductor = FindObjectOfType<Conductor>();
            }
        }

        IEnumerator PlayNote(int note, int delay)
        {
#if AudioHelm
            NoteOn?.Invoke();
            controller.NoteOn((int)pitch);
            yield return new WaitForSeconds(delay);
            NoteOff?.Invoke();
            controller.NoteOff(note);
#endif
            yield return null;
        }

        // Update is called once per frame
        void Update()
        {
            if (volumeOscillator)
                volume = volumeOscillator.GetValue() * ((conductor != null) ? conductor.masterVolume : 1);
            else if (conductor != null)
                volume = conductor.masterVolume;
            if (pitchOscillator)
                pitch = pitchOscillator.GetValue();
            if (panOscillator)
                panStereo = panOscillator.GetValue();

            if (playOscillator || playTrigger)
            {
                float p = 0;
                if (playOscillator)
                    p = playOscillator.GetValue();
                if (playTrigger)
                    p = playTrigger.GetValue();

                if (p > playValue && prevPlayOscillatorCounter < playValue)
                {

                    // if (volumeOscillator)
                    //     audio.volume = volumeOscillator.GetValue() * ((conductor!=null)?conductor.masterVolume:1);
                    // else if(conductor!=null)
                    //     audio.volume = conductor.masterVolume;
                    // if (pitchOscillator)
                    //     audio.pitch = pitchOscillator.GetValue();
                    // if (panOscillator)
                    //     audio.panStereo = panOscillator.GetValue();
                    // controller.NoteOff((int)pitch);
                    // controller.NoteOn((int)pitch);
                    StartCoroutine(PlayNote((int)pitch, 2));
                }
                prevPlayOscillatorCounter = p;
            }
            // else if (audio != null && !audio.isPlaying && forceAudioToPlay)
            // {
            //     audio.Play();
            // }
            // if (crop && audio.isPlaying && audio.time > outTime)
            // {
            //     audio.Stop();
            // }
            // for (int i = 0; i < sources.Count; i++)
            // {
            //     if (crop && sources[i].isPlaying && sources[i].time > outTime)
            //     {
            //         sources[i].Stop();
            //     }
            //     if (!sources[i].isPlaying)
            //     {
            //         Destroy(sources[i].gameObject);
            //         sources.RemoveAt(i);
            //     }
            // }
        }

    }
}