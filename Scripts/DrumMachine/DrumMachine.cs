using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class DrumMachine : MonoBehaviour
{
    public AudioSource hiHatSource;
    public AudioSource snareDrumSource;
    public AudioSource bassDrumSource;

    public float HHOdds = .3f;
    public float SDOdds = .5f;
    public float BDOdds = .5f;

    public int beatsPerBar = 4;
    public int numberOfBars = 4; // Modify this for longer sequences
    public int loopsUntilRandom = 3;
    int loopCounter = 0;

    private DrumTab drumTab;

    public bool randomizeBeat = false; // Flag to trigger beat randomization
    public bool randomizeInstrumets = false;
    public int tempo = 120; // Beats per minute
    public int numberOfBeats = 16; // Total number of beats for the pattern

    public string HiHats;
    public string Snares;
    public string BassDrums;

    public UnityEvent randomizeEvent;
    public UnityEvent bassHitEvent;
    public UnityEvent snareHitEvent;
    public UnityEvent hiHatHitEvent;

    AudioManager audioManager;

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        ReplaceDrumClips(); // Call this method to replace clips at start or whenever appropriate
        GenerateRandomDrumTab();
        StartCoroutine(PlayDrumTab());
    }


    void ReplaceDrumClips()
    {
        hiHatSource.clip = GetRandomClipFromDirectory(HiHats);
        snareDrumSource.clip = GetRandomClipFromDirectory(Snares);
        bassDrumSource.clip = GetRandomClipFromDirectory(BassDrums);
    }

    AudioClip GetRandomClipFromDirectory(string directory)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(directory);
        return clips[Random.Range(0, clips.Length)];
    }

    void GenerateRandomDrumTab()
    {
        Debug.Log("Generating random drum tab");
        drumTab = new DrumTab
        {
            tempo = tempo,
            timeSignature = "4/4",
            beats = new Beat[beatsPerBar * numberOfBars]
        };

        for (int i = 0; i < drumTab.beats.Length; i++)
        {
            bool isDownBeat = i % beatsPerBar == 0;
            bool isBackBeat = (i + 2) % beatsPerBar == 0;
            bool isFill = true;//i >= drumTab.beats.Length - beatsPerBar - 1; // Last bar as fill

            drumTab.beats[i] = new Beat
            {
                HH = (!isFill || Random.value > HHOdds) ? "x" : "-",
                HHVolume = Random.Range(0.3f, 1f),
                SD = isBackBeat || (isFill && Random.value > SDOdds) ? "o" : "-",
                SDVolume = isBackBeat || (isFill && Random.value > 0.5f) ? Random.Range(0.4f, 1f) : 0,
                BD = isDownBeat || (isFill && Random.value > BDOdds) ? "o" : "-",
                BDVolume = isDownBeat || (isFill && Random.value > 0.5f) ? Random.Range(0.4f, 1f) : 0
            };
        }
    }


    IEnumerator PlayDrumTab()
    {
        double startTime = AudioSettings.dspTime;
        double beatDuration = 60.0 / drumTab.tempo;

        foreach (var beat in drumTab.beats)
        {
            if (randomizeBeat)
            {
                Debug.Log("randomizing");
                randomizeBeat = false; // Reset the flag
                if(randomizeInstrumets)
                    ReplaceDrumClips();
                GenerateRandomDrumTab(); // Generate a new random drum tab
                StopAllCoroutines(); // Stop the current playback
                StartCoroutine(PlayDrumTab()); // Start the new playback
                tempo = (int)Random.Range(26, 32) * 10;
                randomizeEvent?.Invoke();
            }
            ScheduleBeat(beat, startTime);
            startTime += beatDuration;
            yield return new WaitForSeconds((float)beatDuration);
            
            if(loopCounter>=loopsUntilRandom){
                loopCounter = 0;
                randomizeBeat = true;
            }
        }
        loopCounter++;
        // Check if randomization was requested
   
            StartCoroutine(PlayDrumTab());
    }

    void ScheduleBeat(Beat beat, double time)
    {
        if(audioManager==null){
            if (beat.HH == "x")
            {
                hiHatSource.volume = beat.HHVolume;
                hiHatSource.PlayScheduled(time);
                InvokeUnityEvent(time - AudioSettings.dspTime + .2f, hiHatHitEvent);
            }
            if (beat.SD == "o")
            {
                snareDrumSource.volume = beat.SDVolume;
                snareDrumSource.PlayScheduled(time);
                InvokeUnityEvent(time - AudioSettings.dspTime + .2f, snareHitEvent);
            }
            if (beat.BD == "o")
            {
                bassDrumSource.volume = beat.BDVolume;
                bassDrumSource.PlayScheduled(time);
                InvokeUnityEvent(time - AudioSettings.dspTime + .2f, bassHitEvent);
            }
        }
        else{
            if (beat.HH == "x")
            {
                audioManager.PlayOneShotScheduled(hiHatSource.clip, (float)time, beat.HHVolume);
                InvokeUnityEvent(time - AudioSettings.dspTime + .1f, hiHatHitEvent);
            }
            if (beat.SD == "o")
            {
                audioManager.PlayOneShotScheduled(snareDrumSource.clip, (float)time, beat.SDVolume);
                InvokeUnityEvent(time - AudioSettings.dspTime + .2f, snareHitEvent);
            }
            if (beat.BD == "o")
            {
                audioManager.PlayOneShotScheduled(bassDrumSource.clip, (float)time, beat.BDVolume);
                InvokeUnityEvent(time - AudioSettings.dspTime + .2f, bassHitEvent);
            }
        }
    }

    void InvokeUnityEvent(double delay, UnityEvent e)
    {
        StartCoroutine(InvokeAfterDelay(delay, e));
    }

    IEnumerator InvokeAfterDelay(double delay, UnityEvent e)
    {
        yield return new WaitForSeconds((float)delay);
        e?.Invoke();
    }
}

[System.Serializable]
public class DrumTab
{
    public int tempo;
    public string timeSignature;
    public Beat[] beats;
}

[System.Serializable]
public class Beat
{
    public string HH;
    public float HHVolume;
    public string SD;
    public float SDVolume;
    public string BD;
    public float BDVolume;
}

