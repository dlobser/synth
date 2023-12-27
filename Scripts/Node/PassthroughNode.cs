using Unity.VisualScripting;

[UnitCategory("ON/PassThrough")]
[UnitTitle("Pass Through")]
public class PassthroughNode : Unit
{
    [DoNotSerialize]
    public ValueInput inputValue; // Input value

    [DoNotSerialize]
    public ValueOutput outputValue; // Output value

    protected override void Definition()
    {
        inputValue = ValueInput<object>("Input", null); // Accept any type of object
        outputValue = ValueOutput("Output", flow => flow.GetValue(inputValue));

        Requirement(inputValue, outputValue); // Connect input to output
    }
}
