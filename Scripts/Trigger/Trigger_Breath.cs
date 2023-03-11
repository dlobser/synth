using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ON.synth
{
    public class Trigger_Breath : Trigger
    {
        public enum States
        {
            BREATHING, INHALE, EXHALE, HOLD
        }
        public States stateForValue;

        float inhaleLerpValue = 0;

        public GameObject head;                 //hmd or camera
        private float headForwardRotation;      //rotation of hmd or camera
        // public GameObject breathSyncObject;     //the object to manipulate using breath

        //calibration loading feedback
        // public GameObject calibrationText;
        private float timer;

        //breath values
        public float low = 0.2f;
        public float high = 1f;
        // private float value;
        public float lazyLerpSpeed = 10f;
        public float lerpedValue;
        private List <float> previousValues = new List<float>();
        private float _previousValue;

        //head rotation detection
        private List<float> headRotValues = new List<float>();
        [SerializeField] private int recalibrateDuration = 300;
        private float maxRotationValue;
        private float minRotationValue;
        private List<float> minValues = new List<float>();
        private List<float> maxValues = new List<float>();
        private float averageMinValue;
        private float averageMaxValue;
        private float delta;

        //breath sin wave
        private bool _isHolding;
        private bool _inhaling;
        // private Transform breathTransform;
        private float x;
        private float y;
        private float _y;
        public float amplitude = 1f;
        // public Text breathFeedback;

        private void Start()
        {
            lerpedValue = 0; 
            _previousValue = 0;
            headForwardRotation = 0;
            minRotationValue = 0; 
            maxRotationValue = 0;
            _isHolding = false;
            timer = 0f; 
            y = 0; 
            _y = 0; 
            x = -1;
            if(head==null){
                head = Camera.main.gameObject;
            }
            // breathTransform = breathSyncObject.GetComponent<Transform>();
        }

        void Update()
        {
            //calibration loading...
            timer += Time.deltaTime;
            // if (timer > recalibrateDuration / 60)
            // {
            //     // calibrationText.SetActive(false);
            //     //breathing in, out and hold
            //     // moveObjectByBreath();
            // }

            //get x-rotation and calibrate min-max
            headForwardRotation = head.transform.rotation.x;
            CalibrateMinMax(headForwardRotation);

            float val = 0;
            //map values and lerp
            val = map(headForwardRotation, averageMinValue, averageMaxValue, low, high);
            val = Mathf.Clamp(val, low, high);
            lerpedValue = Mathf.Lerp(lerpedValue, val, /*Time.deltaTime/*/lazyLerpSpeed);
            if (float.IsNaN(lerpedValue))
                lerpedValue = 0;

            //breathSyncObject.transform.localScale = new Vector3(lerpedValue, lerpedValue, lerpedValue);
            MeasureBreathInAndOut(60); //measure every 60 frames interval


            switch (stateForValue)
            {
                case (States.BREATHING):
                    value = lerpedValue;
                    break;
                case (States.INHALE):
                    inhaleLerpValue = Mathf.Lerp(inhaleLerpValue, ((_inhaling && !_isHolding)?0:1), Time.deltaTime/lazyLerpSpeed);
                    value = inhaleLerpValue;
                    break;
                case (States.EXHALE):
                    inhaleLerpValue = Mathf.Lerp(inhaleLerpValue, ((!_inhaling  && !_isHolding)?0:1), Time.deltaTime/lazyLerpSpeed);
                    value = inhaleLerpValue;
                    break;
                case (States.HOLD):
                    value = ((_isHolding)?0:1);
                    break;
                default:
                    value = val;
                    break;
            }

            value = map(value, 0, 1, 1, 0);
        }

        private void MeasureBreathInAndOut(int interval)
        {
            if (_previousValue != lerpedValue)
            {
                previousValues.Add(_previousValue);
                if (previousValues.Count > interval)
                {
                    previousValues.RemoveAt(0);
                    
                    if (previousValues[0] > previousValues[interval - 1])
                    {
                        _inhaling = true;
                        delta = previousValues[0] - previousValues[interval - 1];
                    }
                    else
                    {
                        _inhaling = false;
                        delta = previousValues[interval - 1] - previousValues[0];
                    }
                    
                    if (delta < 0.04f)
                    {
                        _isHolding = true;
                    } 
                    else
                    {
                        _isHolding = false;
                    }
                }
                _previousValue = lerpedValue;
            }
        }

        public float map(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        void CalibrateMinMax(float value)
        {
            headRotValues.Add(value);
            maxRotationValue = headRotValues.Max();
            minRotationValue = headRotValues.Min();
            minValues.Add(minRotationValue);
            maxValues.Add(maxRotationValue);
            if (headRotValues.Count > recalibrateDuration)
                headRotValues.RemoveAt(0);  
            if (minValues.Count > recalibrateDuration)
                minValues.RemoveAt(0);
            if (maxValues.Count > recalibrateDuration)
                maxValues.RemoveAt(0);

            //average min and max values
            if (minValues.Count == recalibrateDuration && maxValues.Count == recalibrateDuration)
            {
                float sumMin = 0;
                float sumMax = 0;
                for (int i = 0; i < recalibrateDuration; i++)
                {
                    sumMin += minValues[i];
                    sumMax += maxValues[i];
                }
                averageMinValue = sumMin / recalibrateDuration;
                averageMaxValue = sumMax / recalibrateDuration;
            }
        }
    }
}