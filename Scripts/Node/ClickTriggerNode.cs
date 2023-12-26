using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Custom/Control")]
[UnitTitle("Click Trigger")]
public class ClickTriggerNode : Unit
{
    [DoNotSerialize]
    public ControlInput clickInput;

    [DoNotSerialize]
    public ValueOutput triggerOutput;

    private bool triggered;

    protected override void Definition()
    {
        clickInput = ControlInput("click", Clicked);
        triggerOutput = ValueOutput<bool>("triggered", (flow) => triggered);
    }

    private ControlOutput Clicked(Flow flow)
    {
        // Set triggered to true and then immediately back to false
        triggered = true;
        flow.SetValue(triggerOutput, triggered); // Update the value immediately
        triggered = false;

        return null;
    }
}
