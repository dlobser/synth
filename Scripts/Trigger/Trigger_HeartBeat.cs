using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ON.synth
{
    public class Trigger_HeartBeat : Trigger
    {
        List<Vector3> positions;
        public int length = 5;
        List<float> lengths;
        LineRenderer line;
        public int lineLength = 1000;
        public Transform headsetTracker;
        public float beatFadeTime;
        


        
         //public AudioClip song;
    //  public AudioSource song;
     //public GameObject cube;
     private bool Beated;
     private float[] historyBuffer = new float[43];
     private float[] channelRight;
     private float[] channelLeft;
     private int SamplesSize = 1024;
     private float InstantSpec;
     private float AverageSpec;
     private float VarianceSum;
     private float Variance;
     private float Constant;

     List<float> times;
     public float bpm;
     public float hrv;
    
    float timeSince = 0;
     float spec = 0;

    public float minBeatTime = .5f;

    public float interval = 0;


    void StartHeadsetTracking()
    {
        if(GetComponent<LineRenderer>()!=null)
            line = GetComponent<LineRenderer>();
        positions = new List<Vector3>();
        lengths = new List<float>();
        for (int i = 0; i < 20; i++)
        {
            lengths.Add(1);
        }
        times = new List<float>();
    }

    float GetAverageBeatLength(float value){
        times.Add(value);
        if(times.Count>100){
            times.RemoveAt(0);
        }
        float avg = 0;
        List<float> times2 = new List<float>();
        for (int i = 0; i < times.Count; i++)
        {
            times2.Add(times[i]);
        }
        times2.Sort();
        float outAverage = times2[(int)(times2.Count*.5f)];// avg/times.Count;
        // print(60/outAverage);
        bpm = outAverage;
        
        float diff = 0;
        if(times.Count>99){
            // List<float> times3 = new List<float>();
            for (int i = 20; i < 80; i++)
            {
                float a = Mathf.Abs(times[i]-times[i-1]);
                diff+=a;
                // times3.Add(times[i]);
            }
            diff/=60;
        }
        hrv = diff;
        return outAverage;
    }

    // Update is called once per frame
    void UpdateHeadsetTracking()
    {
        positions.Add(headsetTracker.position);
        while(positions.Count > length){
            positions.RemoveAt(0);
        }
        float len = 0;
        float max = -1000;
        for (int i = 0; i < positions.Count-1; i++)
        {
            float d = Vector3.Distance(positions[i],positions[i+1]);
            len += d;
            if(d>max){
                max = d;
            }
        }
        len/=positions.Count;
        lengths.Add(len);
        if(line!=null){
            line.positionCount = lengths.Count;
            for (int i = 0; i < lengths.Count-1; i++)
            {
                line.SetPosition(i,new Vector3((float)i*.01f,lengths[i]*10f,0));
            }
        }
        while(lengths.Count>lineLength){
            lengths.RemoveAt(0);
        }

        InstantSpec = lengths[lengths.Count-1];
        
    }


     
     // Use this for initialization
     void Start () {
        StartHeadsetTracking();
        Beated = false;
     }

     public void SetInstantSpec(float val){
        InstantSpec = val;
     }
     
     // Update is called once per frame
     void Update () {

        UpdateHeadsetTracking();
    
         AverageSpec =  sumLocalEnergy2(historyBuffer);  //Rafa

         //Variance = VarianceSum/historyBuffer.Length;  //Normal
         Variance = VarianceAdder(historyBuffer) / historyBuffer.Length;  //Rafa
         Constant = 1;//(float)((-0.0025714 * Variance) + 1.5142857);  //Normal
         
         float[] shiftingHistoryBuffer = new float[historyBuffer.Length]; // make a new array and copy all the values to it
         
         for (int i = 0; i < (historyBuffer.Length - 1); i++) { // now we shift the array one slot to the right
             shiftingHistoryBuffer[i+1] = historyBuffer[i]; // and fill the empty slot with the new instant sound energy
         }
         
         shiftingHistoryBuffer [0] = InstantSpec;
         
         for (int i = 0; i < historyBuffer.Length; i++) {
             historyBuffer[i] = shiftingHistoryBuffer[i]; //then we return the values to the original array
         }
         
        //  print(InstantSpec + " , " + Constant + " , " + AverageSpec);
         if (InstantSpec > (Constant * AverageSpec)) { // now we check if we have a beat
             if(!Beated) {
                if(InstantSpec>spec && (Time.time-timeSince)>minBeatTime){
                    Debug.Log("Beat");
                    Beated = true;
                    print(Time.time + " , " + timeSince + " , " + (Time.time-timeSince));
                    GetAverageBeatLength(Time.time-timeSince);
                    interval = Mathf.Lerp(interval,Time.time-timeSince,.1f);
                    timeSince = Time.time;

                    value = 1;
                }
                spec = InstantSpec;
                
             }
         } 
         else {
             if(Beated) {
                 Beated = false;
             }
             Debug.Log("No Beat");
         }
         if(spec>0)
            spec-=Time.deltaTime*.001f;
        if(value>0){
            value -= Time.deltaTime * beatFadeTime;
        }
        else{
            value = 0;
        }

     }
     
     float sumStereo(float[] channel1, float[] channel2) {
         float e = 0;
         for (int i = 0; i<channel1.Length; i++) {
             e += ((channel1[i]*channel1[i]) + (channel2[i]*channel2[i]));
         }
         
         return e;
     }
 
     float sumStereo2(float[] Channel) {
         float e = 0;
         for (int i = 0; i < Channel.Length; i++) {
             float ToSquare = Channel[i];
             e += (ToSquare * ToSquare);
         }
         return e;
     }
     
     float sumLocalEnergy() {
         float E = 0;
         
         for (int i = 0; i<historyBuffer.Length; i++) {
             E += historyBuffer[i]*historyBuffer[i];
         }
         
         return E;
     }
 
     float sumLocalEnergy2(float[] Buffer) {
         float E = 0;
         for (int i = 0; i < Buffer.Length; i++) {
             float ToSquare = Buffer[i];
             E += (Buffer[i] * Buffer[i]);
         }
         return E;
     }
 
     float VarianceAdder (float[] Buffer) {
         float VarSum = 0;
         for (int i = 0; i < Buffer.Length; i++) {  //Rafa
             float ToSquare = Buffer[i] - AverageSpec;
             VarSum += (ToSquare * ToSquare);
         }
         return VarSum;
     }
     
     string historybuffer() {
         string s = "";
         for (int i = 0; i<historyBuffer.Length; i++) {
             s += (historyBuffer[i] + ",");
         }
         return s;
     }

    }
}