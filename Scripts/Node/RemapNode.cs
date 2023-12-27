using Unity.VisualScripting;

[UnitCategory("ON/Math")]
[UnitTitle("Remap")]
public class Remap : Unit
{
    [DoNotSerialize]
    public ValueInput Source, SourceMin, SourceMax, TargetMin, TargetMax;

    [DoNotSerialize]
    public ValueOutput MappedValue;

    protected override void Definition()
    {
        Source = ValueInput<float>("Source", 0f);
        SourceMin = ValueInput<float>("SourceMin", 0f);
        SourceMax = ValueInput<float>("SourceMax", 1f);
        TargetMin = ValueInput<float>("TargetMin", 0f);
        TargetMax = ValueInput<float>("TargetMax", 1f);

        MappedValue = ValueOutput<float>("MappedValue", flow =>
        {
            float s = flow.GetValue<float>(Source);
            float a1 = flow.GetValue<float>(SourceMin);
            float a2 = flow.GetValue<float>(SourceMax);
            float b1 = flow.GetValue<float>(TargetMin);
            float b2 = flow.GetValue<float>(TargetMax);

            return Map(s, a1, a2, b1, b2);
        });

        Requirement(Source, MappedValue);
        Requirement(SourceMin, MappedValue);
        Requirement(SourceMax, MappedValue);
        Requirement(TargetMin, MappedValue);
        Requirement(TargetMax, MappedValue);
    }

    private float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
