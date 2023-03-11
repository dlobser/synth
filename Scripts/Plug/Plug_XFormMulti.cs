using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Plug_XFormMulti : MonoBehaviour
    {
        public GameObject target;
        public Oscillator oscillator;
        public Trigger trigger;
        public bool TX,TY,TZ,RX,RY,RZ,SX,SY,SZ;

        float value;

        // Start is called before the first frame update
        void Start()
        {
            if(target==null){
                target = this.gameObject;
            }
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

            if(TX||TY||TZ){
                target.transform.localPosition = new Vector3(
                    TX?value:target.transform.localPosition.x,
                    TY?value:target.transform.localPosition.y,
                    TZ?value:target.transform.localPosition.z
                );
            }
            if(RX||RY||RZ){
                target.transform.localEulerAngles = new Vector3(
                    RX?value:target.transform.localEulerAngles.x,
                    RY?value:target.transform.localEulerAngles.y,
                    RZ?value:target.transform.localEulerAngles.z
                );
            }
            if(SX||SY||SZ){
                target.transform.localScale = new Vector3(
                    SX?value:target.transform.localScale.x,
                    SY?value:target.transform.localScale.y,
                    SZ?value:target.transform.localScale.z
                );
            }
            
        }
    }
}