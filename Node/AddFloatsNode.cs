using Unity.VisualScripting;

[UnitCategory("Custom/SimpleTest")]
[UnitTitle("Add Floats")]
public class AddFloatsNode : Unit
{
    // Define inputs
    [DoNotSerialize] // No need to serialize inputs
    public ValueInput inputA;

    [DoNotSerialize]
    public ValueInput inputB;

    // Define output
    [DoNotSerialize]
    public ValueOutput sum;

    protected override void Definition()
    {
        // Create input ports
        inputA = ValueInput<float>("A", 0.0f);
        inputB = ValueInput<float>("B", 0.0f);

        // Create output port
        sum = ValueOutput<float>("Sum", GetSum);

        // Specify the function to call when the output port is evaluated
        Requirement(inputA, sum);
        Requirement(inputB, sum);
    }

    private float GetSum(Flow flow)
    {
        // Get the values from the input ports and add them
        float a = flow.GetValue<float>(inputA);
        float b = flow.GetValue<float>(inputB);
        return a + b;
    }
}
