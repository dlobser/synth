using Unity.VisualScripting;
using ON.synth;
using UnityEngine;

[UnitCategory("Custom/Audio")]
[UnitTitle("Plug Audio Synth")]
public class PlugAudioSynthNode : Unit
{
    [DoNotSerialize]
    public ValueInput PlugAudioSynthObject;

    [DoNotSerialize]
    public ValueInput TonesArray;

    [DoNotSerialize]
    public ControlOutput output;  // ControlOutput declared at class level

    protected override void Definition()
    {
        PlugAudioSynthObject = ValueInput<Plug_AudioSynth>("PlugAudioSynth", null);
        TonesArray = ValueInput<Plug_AudioSynth.tone[]>("TonesArray", null);

        var triggerInput = ControlInput("set", flow =>
        {
            var plugAudioSynth = flow.GetValue<Plug_AudioSynth>(PlugAudioSynthObject);
            var tones = flow.GetValue<Plug_AudioSynth.tone[]>(TonesArray);

            if (plugAudioSynth != null)
            {
                plugAudioSynth.SetTones(tones);
            }

            return output;  // Return the control output
        });

        output = ControlOutput("output");  // Initialize ControlOutput

        Succession(triggerInput, output);  // Define the flow succession

        Requirement(PlugAudioSynthObject, triggerInput);
        Requirement(TonesArray, triggerInput);
    }
}
