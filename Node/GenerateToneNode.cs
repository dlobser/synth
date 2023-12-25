using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Custom/Audio")]
[UnitTitle("Generate Tone")]
public class GenerateToneNode : Unit
{
    [DoNotSerialize]
    public ValueInput GenerateToneObject;

    [DoNotSerialize]
    public ValueInput Volume;

    [DoNotSerialize]
    public ValueInput Frequency;

    [DoNotSerialize]
    public ValueInput Pan;

    [DoNotSerialize]
    public ValueInput UseNoise;

    [DoNotSerialize]
    public ValueInput UseCustomCurve;

    [DoNotSerialize]
    public ValueInput Curve;

    protected override void Definition()
    {
        GenerateToneObject = ValueInput<GenerateTone>("GenerateTone", null);
        Volume = ValueInput<float>("Volume", 1.0f);
        Frequency = ValueInput<float>("Frequency", 440f);
        Pan = ValueInput<float>("Pan", 0f);
        UseNoise = ValueInput<bool>("UseNoise", false);
        UseCustomCurve = ValueInput<bool>("UseCustomCurve", false);
        Curve = ValueInput<AnimationCurve>("Curve", new AnimationCurve());

        var triggerInput = ControlInput("set", flow =>
        {
            var generateTone = flow.GetValue<GenerateTone>(GenerateToneObject);
            if (generateTone != null)
            {
                generateTone.volume = flow.GetValue<float>(Volume);
                generateTone.frequency = flow.GetValue<float>(Frequency);
                generateTone.pan = flow.GetValue<float>(Pan);
                generateTone.useNoise = flow.GetValue<bool>(UseNoise);
                generateTone.useCustomCurve = flow.GetValue<bool>(UseCustomCurve);
                generateTone.curve = flow.GetValue<AnimationCurve>(Curve);
            }

            return null; // Since this is a control input, it doesn't return a value
        });

        var output = ControlOutput("output");

        Succession(triggerInput, output);

        Requirement(GenerateToneObject, triggerInput);
        Requirement(Volume, triggerInput);
        Requirement(Frequency, triggerInput);
        Requirement(Pan, triggerInput);
        Requirement(UseNoise, triggerInput);
        Requirement(UseCustomCurve, triggerInput);
        Requirement(Curve, triggerInput);
    }

}
