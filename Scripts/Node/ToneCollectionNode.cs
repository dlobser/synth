using Unity.VisualScripting;
using ON.synth;
using UnityEngine;

[UnitCategory("Custom/Audio")]
[UnitTitle("Aggregate Tones")]
public class ToneCollectionNode : Unit
{
    [DoNotSerialize]
    public ValueInput NumberOfTones; // Input for the number of tones to use

    [DoNotSerialize]
    public ValueInput[] ToneInputs; // Array of ValueInput for tones

    [DoNotSerialize]
    public ValueOutput TonesOutput;

    protected override void Definition()
    {
        int maxTones = 10; // Maximum number of tones
        ToneInputs = new ValueInput[maxTones];

        NumberOfTones = ValueInput<int>("NumberOfTones", 5); // Default to 5 tones

        TonesOutput = ValueOutput<Plug_AudioSynth.tone[]>("Tones", flow =>
        {
            int numTones = Mathf.Clamp(flow.GetValue<int>(NumberOfTones), 0, maxTones);
            var tones = new Plug_AudioSynth.tone[numTones];
            for (int i = 0; i < numTones; i++)
            {
                tones[i] = flow.GetValue<Plug_AudioSynth.tone>(ToneInputs[i]);
            }
            return tones;
        });

        for (int i = 0; i < maxTones; i++)
        {
            ToneInputs[i] = ValueInput<Plug_AudioSynth.tone>($"Tone{i + 1}", default(Plug_AudioSynth.tone));
            Requirement(ToneInputs[i], TonesOutput);
        }
    }
}
