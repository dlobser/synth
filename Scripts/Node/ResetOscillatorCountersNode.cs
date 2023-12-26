using Unity.VisualScripting;
using ON.synth;
using UnityEngine;

[UnitCategory("Custom/Oscillator")]
[UnitTitle("Reset Oscillator Counters")]
public class ResetOscillatorCountersNode : Unit
{
    [DoNotSerialize]
    public ValueInput resetTrigger;

    [DoNotSerialize]
    public ControlOutput resetCompleted;

    protected override void Definition()
    {
        resetTrigger = ValueInput<bool>("ResetTrigger", false);

        resetCompleted = ControlOutput("ResetCompleted");

        // Define a control input that executes the reset action
        var triggerInput = ControlInput("reset", (flow) =>
        {
            if (flow.GetValue<bool>(resetTrigger))
            {
                OscillatorEvents.ResetCounters(); // Reset the counters
                flow.SetValue(resetTrigger, false); // Reset the trigger to false
                return resetCompleted;
            }
            return null;
        });

        // Define the flow succession
        Succession(triggerInput, resetCompleted);

        // Set the requirement
        Requirement(resetTrigger, triggerInput);
    }
}
