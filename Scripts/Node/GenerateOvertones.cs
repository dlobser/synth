using Unity.VisualScripting;
using ON.synth;
using UnityEngine;
using System.Collections.Generic;

[UnitCategory("ON/Audio")]
[UnitTitle("Generate Overtones")]
public class GenerateOvertones : Unit
{
    [DoNotSerialize]
    public ValueInput InitialVolume, InitialFrequency, NumberOfTones;

    [DoNotSerialize]
    public ValueOutput TonesOutput;

    protected override void Definition()
    {
        InitialVolume = ValueInput<float>("InitialVolume", 1.0f);
        InitialFrequency = ValueInput<float>("InitialFrequency", 440f);
        NumberOfTones = ValueInput<int>("NumberOfTones", 5);

        TonesOutput = ValueOutput<List<Plug_AudioSynth.tone>>("Tones", flow =>
        {
            var tones = new List<Plug_AudioSynth.tone>();
            float currentVolume = flow.GetValue<float>(InitialVolume);
            float currentFrequency = flow.GetValue<float>(InitialFrequency);
            int numTones = flow.GetValue<int>(NumberOfTones);

            for (int i = 1; i <= numTones; i++)
            {
                var tone = new Plug_AudioSynth.tone
                {
                    volume = currentVolume,
                    frequency = currentFrequency * i, // Multiplying the frequency
                    pan = 0f, // Default value, can be changed as needed
                    advanced = new Plug_AudioSynth.Advanced() // Assuming default or empty Advanced settings
                };

                tones.Add(tone);
                currentVolume /= 2; // Halving the volume for the next tone
            }

            return tones;
        });

        Requirement(InitialVolume, TonesOutput);
        Requirement(InitialFrequency, TonesOutput);
        Requirement(NumberOfTones, TonesOutput);
    }
}
