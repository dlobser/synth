using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
    public class Trigger_Basic : Trigger
    {
        public float low;
        public float high;
        public string key;
        public float attackSpeed = 1000;
        public float releaseSpeed = 1000;
        float lazyValue;

        void Start()
        {
            
        }

        void Update()
        {
            if(key!=""){
                if(Input.GetKey(key)){
                    if(lazyValue<high)
                        lazyValue+=Time.deltaTime*attackSpeed;
                    lazyValue = Mathf.Clamp(lazyValue,low,high);
                    value = Mathf.Lerp(low,high,lazyValue);
                }
                else{
                    if(lazyValue>low)
                        lazyValue -= Time.deltaTime*releaseSpeed;
                    lazyValue = Mathf.Clamp(lazyValue,low,high);
                    value = Mathf.Lerp(low,high,lazyValue);
                }
            }
        }

        public override float GetValue(){
            return value;
        }
    }
}