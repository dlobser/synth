using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Conductor : MonoBehaviour
    {
        public float masterVolume;
        public Oscillator masterVolumeOscillator;

        public float masterSpeed;
        public Oscillator masterSpeedOscillator;

        void Start()
        {
            
        }

        void Update()
        {
            if (masterVolumeOscillator != null)
                masterVolume = masterVolumeOscillator.GetValue();
            if (masterSpeedOscillator != null)
                masterSpeed = masterSpeedOscillator.GetValue();
        }
    }
}