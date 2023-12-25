using Unity.VisualScripting;
using ON.synth;
using UnityEngine;

[UnitCategory("Custom/Audio")]
[UnitTitle("Generate Single Tone")]
public class ToneNode : Unit
{
    [DoNotSerialize]
    public ValueInput Volume, Frequency, Pan, UseNoise, UseCustomCurve;

    [DoNotSerialize]
    public ValueInput Curve;

    [DoNotSerialize]
    public ValueOutput ToneOutput;

    protected override void Definition()
    {
        Volume = ValueInput<float>("Volume", 1.0f);
        Frequency = ValueInput<float>("Frequency", 440f);
        Pan = ValueInput<float>("Pan", 0f);
        UseNoise = ValueInput<bool>("UseNoise", false);
        UseCustomCurve = ValueInput<bool>("UseCustomCurve", false);
        Curve = ValueInput<AnimationCurve>("Curve", new AnimationCurve());

        ToneOutput = ValueOutput<Plug_AudioSynth.tone>("Tone", flow =>
        {
            var tone = new Plug_AudioSynth.tone
            {
                volume = flow.GetValue<float>(Volume),
                frequency = flow.GetValue<float>(Frequency),
                pan = flow.GetValue<float>(Pan),
                useNoise = flow.GetValue<bool>(UseNoise),
                useCustomCurve = flow.GetValue<bool>(UseCustomCurve),
                curve = flow.GetValue<AnimationCurve>(Curve)
            };
            return tone;
        });

        Requirement(Volume, ToneOutput);
        Requirement(Frequency, ToneOutput);
        Requirement(Pan, ToneOutput);
        Requirement(UseNoise, ToneOutput);
        Requirement(UseCustomCurve, ToneOutput);
        Requirement(Curve, ToneOutput);
    }
}
