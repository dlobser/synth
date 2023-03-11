using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Plug_XForm : MonoBehaviour
    {
        public GameObject target;
        public Oscillator oscillator;
        public Trigger trigger;

        public enum XForm{TX,TY,TZ,RX,RY,RZ,SX,SY,SZ};
        public XForm xForm;

        float value;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (oscillator != null){
                value = oscillator.GetValue();
                if (trigger != null)
                    value *= trigger.GetValue();
            }
            else if (trigger != null)
                value = trigger.GetValue() ;

            switch (xForm)
            {
                case XForm.TX:
                    target.transform.localPosition = new Vector3(value,target.transform.localPosition.y,target.transform.localPosition.z);
                    break;
                case XForm.TY:
                    target.transform.localPosition = new Vector3(target.transform.localPosition.x,value,target.transform.localPosition.z);
                    break;
                case XForm.TZ:
                    target.transform.localPosition = new Vector3(target.transform.localPosition.x,target.transform.localPosition.y,value);
                    break;
                default:
                    break;
            }
        }
    }
}