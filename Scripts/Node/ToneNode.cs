using Unity.VisualScripting;
using ON.synth;
using UnityEngine;

[UnitCategory("ON/Audio")]
[UnitTitle("Tone")]
public class ToneNode : Unit
{
    [DoNotSerialize]
    public ValueInput Volume, Frequency, Pan, Doppler, UseNoise, UseCustomCurve;

    [DoNotSerialize]
    public ValueInput Curve;

    [DoNotSerialize]
    public ValueOutput ToneOutput;

    protected override void Definition()
    {
        Volume = ValueInput<float>("Volume", 1.0f);
        Frequency = ValueInput<float>("Frequency", 440f);
        Pan = ValueInput<float>("Pan", 0f);
        Doppler = ValueInput<float>("Doppler", 0f);
        UseNoise = ValueInput<bool>("UseNoise", false);
        UseCustomCurve = ValueInput<bool>("UseCustomCurve", false);
        Curve = ValueInput<AnimationCurve>("Curve", new AnimationCurve());

        ToneOutput = ValueOutput<Plug_AudioSynth.tone>("Tone", flow =>
        {
            var advanced = new Plug_AudioSynth.Advanced
            {
                doppler = flow.GetValue<float>(Doppler),
                useNoise = flow.GetValue<bool>(UseNoise),
                useCustomCurve = flow.GetValue<bool>(UseCustomCurve),
                curve = flow.GetValue<AnimationCurve>(Curve),
                oscillators = new Plug_AudioSynth.Oscillators() // Assuming default or empty Oscillators for now
            };

            var tone = new Plug_AudioSynth.tone
            {
                volume = flow.GetValue<float>(Volume),
                frequency = flow.GetValue<float>(Frequency),
                pan = flow.GetValue<float>(Pan),
                advanced = advanced
            };
            return tone;
        });

        Requirement(Volume, ToneOutput);
        Requirement(Frequency, ToneOutput);
        Requirement(Pan, ToneOutput);
        Requirement(Doppler, ToneOutput);
        Requirement(UseNoise, ToneOutput);
        Requirement(UseCustomCurve, ToneOutput);
        Requirement(Curve, ToneOutput);
    }
}
