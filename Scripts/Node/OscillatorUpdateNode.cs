using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("ON/Oscillator")]
[UnitTitle("Oscillator Update")]
public class OscillatorUpdateNode : Unit
{
    [DoNotSerialize]
    public ControlInput triggerInput;

    [DoNotSerialize]
    public ControlOutput triggerOutput;

    protected override void Definition()
    {
        triggerInput = ControlInput("trigger", TriggerUpdateEvent);
        triggerOutput = ControlOutput("output");

        Succession(triggerInput, triggerOutput);
    }

    private ControlOutput TriggerUpdateEvent(Flow flow)
    {
        OscillatorEvents.UpdateEvent();
        return triggerOutput;
    }
}
