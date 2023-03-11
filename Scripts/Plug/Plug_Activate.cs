using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ON.synth
{
    public class Plug_Activate : MonoBehaviour
    {
        public GameObject[] targets;
        public Oscillator oscillator;
        public Trigger trigger;

        [Tooltip("Deactivate when outside the range")]
        public Vector2 MinMaxValue;
        public bool deactivateInsideRange;
        bool prevActive;

        void OnEnable()
        {
            float value = Synth_Util.GetOscTrigValue(oscillator, trigger);
            bool active = (value < MinMaxValue.y && value > MinMaxValue.x);
            foreach (GameObject g in targets)
            {
                g.SetActive(!deactivateInsideRange ? active : !active);
            }
            prevActive = active;
        }

        void Update()
        {
            float value = Synth_Util.GetOscTrigValue(oscillator, trigger);
            bool active = (value < MinMaxValue.y && value > MinMaxValue.x);
            if (active != prevActive)
            {
                foreach (GameObject g in targets)
                {
                    g.SetActive(!deactivateInsideRange ? active : !active);
                }
            }
            prevActive = active;
        }
    }
}