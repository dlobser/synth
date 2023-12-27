using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;

[UnitCategory("Custom/Audio")]
[UnitTitle("Play Audio Scheduled")]
public class PlayAudioScheduledNode : Unit
{
    [DoNotSerialize]
    public ValueInput audioSourcesInput; // List of AudioSources
    [DoNotSerialize]
    public ValueInput intervalInput;
    [DoNotSerialize]
    public ValueInput pitchInput; // New input for pitch
    [DoNotSerialize]
    public ValueInput volumeInput; // New input for volume
    [DoNotSerialize]
    public ValueInput panInput; // New input for pan (panStereo)

    [DoNotSerialize]
    public ControlInput execute;
    [DoNotSerialize]
    public ControlOutput playedOutput;

     [DoNotSerialize]
    public ControlOutput triggerOutput;

    private double nextPlayTime = 0;
    private int currentIndex = 0;

    protected override void Definition()
    {
        audioSourcesInput = ValueInput<List<AudioSource>>("AudioSources", null);
        intervalInput = ValueInput<float>("Interval", 1f);
        pitchInput = ValueInput<float>("Pitch", 1f); // Default pitch is 1
        volumeInput = ValueInput<float>("Volume", 1f); // Default volume is 1
        panInput = ValueInput<float>("Pan", 0f); // Default pan is centered (0)
            execute = ControlInput("", PlayAudio);
        playedOutput = ControlOutput("");
        triggerOutput = ControlOutput("Played");

    

        Requirement(audioSourcesInput, execute);
        Requirement(intervalInput, execute);
        Requirement(pitchInput, execute);
        Requirement(volumeInput, execute);
        Requirement(panInput, execute);
        Succession(execute, playedOutput);
    }

    private ControlOutput PlayAudio(Flow flow)
    {
        List<AudioSource> audioSources = flow.GetValue<List<AudioSource>>(audioSourcesInput);
        float interval = flow.GetValue<float>(intervalInput);
        float pitch = flow.GetValue<float>(pitchInput);
        float volume = flow.GetValue<float>(volumeInput);
        float pan = flow.GetValue<float>(panInput);

        double currentTime = AudioSettings.dspTime;

        if (audioSources != null && audioSources.Count > 0 && currentIndex < audioSources.Count)
        {
            AudioSource currentAudioSource = audioSources[currentIndex];
            if (currentAudioSource != null && currentAudioSource.clip != null)
            {
                if (currentTime >= nextPlayTime)
                {
                    currentAudioSource.pitch = pitch;
                    currentAudioSource.volume = volume;
                    currentAudioSource.panStereo = pan;

                    currentAudioSource.PlayScheduled(nextPlayTime);
                    nextPlayTime = currentTime + interval;

                    currentIndex = (currentIndex + 1) % audioSources.Count;

                    flow.Invoke(triggerOutput);
                }
            }
        }

        return playedOutput;
    }
}
