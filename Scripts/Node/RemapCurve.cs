using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Custom/Math")]
[UnitTitle("Remap Curve")]
public class RemapCurve : Unit
{
    [DoNotSerialize]
    public ValueInput Source, SourceMin, SourceMax, Curve;

    [DoNotSerialize]
    public ValueOutput SampledValue;

    protected override void Definition()
    {
        Source = ValueInput<float>("Source", 0f);
        SourceMin = ValueInput<float>("SourceMin", 0f);
        SourceMax = ValueInput<float>("SourceMax", 1f);
        Curve = ValueInput<AnimationCurve>("Curve", new AnimationCurve());

        SampledValue = ValueOutput<float>("SampledValue", flow =>
        {
            float s = flow.GetValue<float>(Source);
            float a1 = flow.GetValue<float>(SourceMin);
            float a2 = flow.GetValue<float>(SourceMax);
            AnimationCurve curve = flow.GetValue<AnimationCurve>(Curve);

            float remappedValue = Remap(s, a1, a2, 0f, 1f);
            return curve.Evaluate(remappedValue);
        });

        Requirement(Source, SampledValue);
        Requirement(SourceMin, SampledValue);
        Requirement(SourceMax, SampledValue);
        Requirement(Curve, SampledValue);
    }

    private float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
