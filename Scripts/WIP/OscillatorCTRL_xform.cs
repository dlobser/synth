using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ON.synth{
public class OscillatorCTRL_xform : MonoBehaviour
{
    public Oscillator oscillatorX;
    public Oscillator oscillatorY;
    public Oscillator oscillatorZ;
    public Oscillator oscillatorUniform;

    public Trigger triggerX;
    public Trigger triggerY;
    public Trigger triggerZ;
    public Trigger triggerUniform;

    public bool doTranslate;
    public bool doRotate;
    public bool doScale;

    Vector3 translate, rotate, scale;

    float x,y,z,u;
    public float addX,addY,addZ,addUniform;

    public Transform target;

    void Start()
    {
        if(target==null)
            target = this.transform;
    }


    void Update()
    {
        if(oscillatorX){
            x = oscillatorX.GetValue();
            if(triggerX)
                x*=triggerX.GetValue();
        }
        else if(triggerX)
            x=triggerX.GetValue();
        else
            x = 0;

        if(oscillatorY){
            y = oscillatorY.GetValue();
            if(triggerY)
                y*=triggerY.GetValue();
        }
        else if(triggerY)
            y=triggerY.GetValue();
        else
            y = 0;

        if(oscillatorZ){
            z = oscillatorZ.GetValue();
            if(triggerZ)
                z *= triggerZ.GetValue();
        }
        else if(triggerZ)
            z = triggerZ.GetValue();
        else
            z = 0;
        
        if(oscillatorUniform){
            u = oscillatorUniform.GetValue();
            if(triggerUniform)
                u *= triggerZ.GetValue();
        }
        else if(triggerUniform)
            u = triggerUniform.GetValue();
        else
            u = 0;

        x+=addX;
        y+=addY;
        z+=addZ;
        u+=addUniform;
        
        if(doTranslate){
            translate.Set(x,y,z);
            if(oscillatorUniform||triggerUniform)
                translate.Set(u,u,u);
            target.localPosition = translate;
        }
        if(doRotate){
            rotate.Set(x,y,z);
            if(oscillatorUniform||triggerUniform)
                rotate.Set(u,u,u);
            target.localEulerAngles = rotate;
        }
        if(doScale){
            scale.Set(x,y,z);
            if(oscillatorUniform||triggerUniform)
                scale.Set(u,u,u);
            target.localScale = scale;
        }
    }
}
}