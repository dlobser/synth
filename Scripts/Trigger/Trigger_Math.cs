using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth
{
    public class Trigger_Math : Trigger
    {

        public Trigger triggerA;
        public Oscillator oscillatorA;

        public Trigger triggerB;
        public Oscillator oscillatorB;

        public enum Operator { ADD, MULTIPLY, DIVIDE };
        public Operator mathOperator;

        public string outValue;
        public bool autoUpdate = false;

        private void Update()
        {
            if (autoUpdate)
            {
                GetValue();
            }
        }

        public override float GetValue()
        {
            float valueA = ON.synth.Synth_Util.GetOscTrigValue(oscillatorA, triggerA);
            float valueB = ON.synth.Synth_Util.GetOscTrigValue(oscillatorB, triggerB);

            if (mathOperator == Operator.ADD)
            {
                value = valueA + valueB;
            }
            else if (mathOperator == Operator.MULTIPLY)
            {
                value = valueA * valueB;
            }
            else
            {
                value = valueA / valueB;
            }

            outValue = value.ToString();

            return value;
        }
    }
}