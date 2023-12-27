using Unity.VisualScripting;
using ON.synth;
using UnityEngine;


[UnitCategory("ON/Oscillator")]
public class OscillatorLFONode : Unit
{
    
    public float trough = -1;
    public float crest = 1;
    public float clampLow = -1000;
    public float clampHigh = 1000;
    public float offset;
    public float timeOffset;
    public float multiply = 1;
    public float speed = 1;
    public float counterPower = 1;
    public float sinPower = 1;
    public float quantize = 0;

    float counter;
    float prevCounter;
    public bool useNoise;
    public bool dontLoopCustomCurve;
    public bool useCustom;
    public bool resetAllCounters;

    public float lazyValue = 0;
    public float lazyLerpSpeed = 1000;
    public AnimationCurve curve;
    public bool debug;
    ValueOutput value;
    private ValueInput _trough;
    private ValueInput _crest;
    private ValueInput _clampLow;
    private ValueInput _clampHigh;
    private ValueInput _offset;
    private ValueInput _timeOffset;
    private ValueInput _multiply;
    private ValueInput _speed;
    private ValueInput _counterPower;
    private ValueInput _sinPower;
    private ValueInput _quantize;

    private ValueInput _useNoise;
    private ValueInput _dontLoopCustomCurve;
    private ValueInput _useCustom;
    private ValueInput _curve;

    public ControlInput clickInput;

    [DoNotSerialize]
    private ValueInput _resetAllCounters;

    PerlinNoise pNoise;

    // Define inputs, outputs, and any necessary variables
    protected override void Definition()
    {

        _trough = ValueInput<float>("Trough", -1);
        _crest = ValueInput<float>("Crest", 1);
        _clampLow = ValueInput<float>("ClampLow", -1000);
        _clampHigh = ValueInput<float>("ClampHigh", 1000);
        _offset = ValueInput<float>("Offset", 0);
        _timeOffset = ValueInput<float>("TimeOffset", 0);
        _multiply = ValueInput<float>("Multiply", 1);
        _speed = ValueInput<float>("Speed", 1);
        _counterPower = ValueInput<float>("CounterPower", 1);
        _sinPower = ValueInput<float>("SinPower", 1);
        _quantize = ValueInput<float>("Quantize", 0);

        _useNoise = ValueInput<bool>("UseNoise", false);
        _dontLoopCustomCurve = ValueInput<bool>("DontLoopCustomCurve", false);
        _useCustom = ValueInput<bool>("UseCustom", false);
        _curve = ValueInput<AnimationCurve>("Curve", new AnimationCurve());
        _resetAllCounters = ValueInput<bool>("ResetAllCounters", false);
        
        value = ValueOutput<float>("SineWave", (flow) => GetSineWave(flow));
        
        Requirement(_speed, value);
        Requirement(_trough, value);
        Requirement(_crest, value);
        Requirement(_clampLow, value);
        Requirement(_clampHigh, value);
        Requirement(_offset, value);
        Requirement(_timeOffset, value);
        Requirement(_multiply, value);
        Requirement(_speed, value);
        Requirement(_counterPower, value);
        Requirement(_sinPower, value);
        Requirement(_quantize, value);
        Requirement(_resetAllCounters, value);

        OscillatorEvents.OnResetCounters += ResetCounter;
        OscillatorEvents.OnUpdateEvent += OnUpdate;
    }

    void OnUpdate()
    {
        this.counter += Time.deltaTime/speed;// * (speed * masterSpeed != 0 ? (1 / (masterSpeed * speed)) : 0);
        if (!useNoise && !dontLoopCustomCurve)
            this.counter = this.counter % 1;
        
    }


    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        OscillatorEvents.OnResetCounters -= ResetCounter;
        OscillatorEvents.OnUpdateEvent -= OnUpdate;
    }

    void ResetAllCounters(){
        OscillatorEvents.ResetCounters();
    }


    private float GetSineWave(Flow flow)
    {

        // float masterSpeed = 1;//conductor != null ? conductor.masterSpeed : 1;
        // this.counter += Time.deltaTime/speed;// * (speed * masterSpeed != 0 ? (1 / (masterSpeed * speed)) : 0);
        // if (!useNoise && !dontLoopCustomCurve)
        //     this.counter = this.counter % 1;

        // Retrieve values from the flow using the respective ValueInput fields
        float troughValue = flow.GetValue<float>(_trough);
        float crestValue = flow.GetValue<float>(_crest);
        float clampLowValue = flow.GetValue<float>(_clampLow);
        float clampHighValue = flow.GetValue<float>(_clampHigh);
        float offsetValue = flow.GetValue<float>(_offset);
        float timeOffsetValue = flow.GetValue<float>(_timeOffset);
        float multiplyValue = flow.GetValue<float>(_multiply);
        float speedValue = flow.GetValue<float>(_speed);
        float counterPowerValue = flow.GetValue<float>(_counterPower);
        float sinPowerValue = flow.GetValue<float>(_sinPower);
        float quantizeValue = flow.GetValue<float>(_quantize);
        bool useNoiseValue = flow.GetValue<bool>(_useNoise);
        bool dontLoopCustomCurveValue = flow.GetValue<bool>(_dontLoopCustomCurve);
        bool useCustomValue = flow.GetValue<bool>(_useCustom);
        bool resetAllCountersValue = flow.GetValue<bool>(_resetAllCounters);
        AnimationCurve curveValue = flow.GetValue<AnimationCurve>(_curve);


        // Use these values in your calculations
        // For example, setting the class properties:
        trough = troughValue;
        crest = crestValue;
        clampLow = clampLowValue;
        clampHigh = clampHighValue;
        offset = offsetValue;
        timeOffset = timeOffsetValue;
        multiply = multiplyValue;
        speed = speedValue;
        counterPower = counterPowerValue;
        sinPower = sinPowerValue;
        quantize = quantizeValue;
        useNoise = useNoiseValue;
        dontLoopCustomCurve = dontLoopCustomCurveValue;
        useCustom = useCustomValue;
        curve = curveValue;
        resetAllCounters = resetAllCountersValue;

        if (resetAllCounters)
        {
            ResetAllCounters();
            resetAllCounters = false;  // Reset the flag
        }

        // Now use these updated values to calculate and return the sine wave value
        return GetValue(); // Assuming GetValue() uses the updated properties
    }

    public void Start()
    {
        counter = 0;
        pNoise = new PerlinNoise(0);
    }


    private void ResetCounter()
    {
        counter = 0;
        Debug.Log("Counter Reset");
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public float GetValue()
    {
        float s;

        float qcounter = counter;

        if (quantize > 0)
        {
            qcounter *= quantize;
            qcounter = Mathf.Floor(qcounter);
            qcounter /= quantize;
        }

        if(pNoise==null)
            pNoise = new PerlinNoise(0);

        if (useNoise && pNoise!=null)
            s = ((float)pNoise.Noise(Mathf.Pow(qcounter, counterPower) + (timeOffset), 0,0)*.95f)+.5f;// Mathf.PerlinNoise(Mathf.Pow(qcounter, counterPower) + (timeOffset), 0);
        else if (useCustom)
            s = curve.Evaluate((Mathf.Pow(qcounter, counterPower) + (timeOffset))%1);
        else
            s = Mathf.Cos(Mathf.Pow(qcounter, counterPower) * Mathf.PI * 2 + (timeOffset % 1) * Mathf.PI * 2);

        float lBound = -1f;

        if (useNoise || useCustom)
            lBound = 0;

        float output = Mathf.Clamp(map(s, lBound, 1, trough, crest) * multiply + offset, clampLow, clampHigh);
 
        output = Mathf.Pow(output, output > 0 ? sinPower : 1);

        lazyValue = Mathf.Lerp(lazyValue, output, lazyLerpSpeed * Time.deltaTime);
        return lazyValue;
    }

}
