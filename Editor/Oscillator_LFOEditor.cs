using UnityEngine;
using UnityEditor;
using ON.synth;

[CustomEditor(typeof(Oscillator_LFO))]
[CanEditMultipleObjects]
public class Oscillator_LFOEditor : Editor
{
    private bool showValues = true;
    private bool showOscillators = false;
    private bool showTriggers = false;
    private bool showAlternateCurve = false;
    private bool showLazy = false;
    private bool showLineDisplay = false;

    public override void OnInspectorGUI()
    {
        Oscillator_LFO script = (Oscillator_LFO)target;
        Undo.RecordObject(script, "Oscillator LFO Change");

        // Non-"Values" fields
        script.info = EditorGUILayout.TextField("Info", script.info);
        script.displayCurve = EditorGUILayout.CurveField("Wave Display", script.displayCurve, GUILayout.Height(50));
        script.displayCurveRange = EditorGUILayout.FloatField("Display Range", script.displayCurveRange);
        // Reset All Counters button
        if (GUILayout.Button("Reset All Counters"))
        {
            script.ResetAllCounters();
        }
        // script.resetAllCounters = EditorGUILayout.Toggle("Reset All Counters", script.resetAllCounters);

        // "Values" section
        showValues = EditorGUILayout.Foldout(showValues, "Values");
        if (showValues)
        {
            EditorGUI.indentLevel++;
            script.trough = EditorGUILayout.FloatField("Trough", script.trough);
            script.crest = EditorGUILayout.FloatField("Crest", script.crest);
            script.clampLow = EditorGUILayout.FloatField("Clamp Low", script.clampLow);
            script.clampHigh = EditorGUILayout.FloatField("Clamp High", script.clampHigh);
            script.offset = EditorGUILayout.FloatField("Offset", script.offset);
            script.timeOffset = EditorGUILayout.FloatField("Time Offset", script.timeOffset);
            script.multiply = EditorGUILayout.FloatField("Multiply", script.multiply);
            script.speed = EditorGUILayout.FloatField("Speed", script.speed);
            script.counterPower = EditorGUILayout.FloatField("Counter Power", script.counterPower);
            script.sinPower = EditorGUILayout.FloatField("Sin Power", script.sinPower);
            script.quantize = EditorGUILayout.FloatField("Quantize", script.quantize);
            EditorGUI.indentLevel--;
        }

        // "Alternate Curve" section
        showAlternateCurve = EditorGUILayout.Foldout(showAlternateCurve, "Alternate Curve");
        if (showAlternateCurve)
        {
            EditorGUI.indentLevel++;
            script.useNoise = EditorGUILayout.Toggle("Use Noise", script.useNoise);
            script.dontLoopCustomCurve = EditorGUILayout.Toggle("Don't Loop Custom Curve", script.dontLoopCustomCurve);
            script.useCustom = EditorGUILayout.Toggle("Use Custom", script.useCustom);
            script.curve = EditorGUILayout.CurveField("Curve", script.curve);
            EditorGUI.indentLevel--;
        }

        // "Lazy" section
        showLazy = EditorGUILayout.Foldout(showLazy, "Lazy");
        if (showLazy)
        {
            EditorGUI.indentLevel++;
            script.lazyValue = EditorGUILayout.FloatField("Lazy Value", script.lazyValue);
            script.lazyLerpSpeed = EditorGUILayout.FloatField("Lazy Lerp Speed", script.lazyLerpSpeed);
            EditorGUI.indentLevel--;
        }

        showOscillators = EditorGUILayout.Foldout(showOscillators, "Oscillators");
        if (showOscillators)
        {
            EditorGUI.indentLevel++;
            script.oscillators.multiplyOscillate = (Oscillator)EditorGUILayout.ObjectField("Multiply Oscillate", script.oscillators.multiplyOscillate, typeof(Oscillator), true);
            script.oscillators.speedOscillate = (Oscillator)EditorGUILayout.ObjectField("Speed Oscillate", script.oscillators.speedOscillate, typeof(Oscillator), true);
            script.oscillators.offsetOscillate = (Oscillator)EditorGUILayout.ObjectField("Offset Oscillate", script.oscillators.offsetOscillate, typeof(Oscillator), true);
            script.oscillators.troughOscillate = (Oscillator)EditorGUILayout.ObjectField("Trough Oscillate", script.oscillators.troughOscillate, typeof(Oscillator), true);
            script.oscillators.crestOscillate = (Oscillator)EditorGUILayout.ObjectField("Crest Oscillate", script.oscillators.crestOscillate, typeof(Oscillator), true);
            script.oscillators.clampLowOscillate = (Oscillator)EditorGUILayout.ObjectField("Clamp Low Oscillate", script.oscillators.clampLowOscillate, typeof(Oscillator), true);
            script.oscillators.clampHighOscillate = (Oscillator)EditorGUILayout.ObjectField("Clamp High Oscillate", script.oscillators.clampHighOscillate, typeof(Oscillator), true);
            script.oscillators.timeOffsetOscillate = (Oscillator)EditorGUILayout.ObjectField("Time Offset Oscillate", script.oscillators.timeOffsetOscillate, typeof(Oscillator), true);
            EditorGUI.indentLevel--;
        }

        showTriggers = EditorGUILayout.Foldout(showTriggers, "Triggers");
        if (showTriggers)
        {
            EditorGUI.indentLevel++;
            script.triggers.multiplyTrigger = (Trigger)EditorGUILayout.ObjectField("Multiply Trigger", script.triggers.multiplyTrigger, typeof(Trigger), true);
            script.triggers.speedTrigger = (Trigger)EditorGUILayout.ObjectField("Speed Trigger", script.triggers.speedTrigger, typeof(Trigger), true);
            script.triggers.offsetTrigger = (Trigger)EditorGUILayout.ObjectField("Offset Trigger", script.triggers.offsetTrigger, typeof(Trigger), true);
            script.triggers.troughTrigger = (Trigger)EditorGUILayout.ObjectField("Trough Trigger", script.triggers.troughTrigger, typeof(Trigger), true);
            script.triggers.crestTrigger = (Trigger)EditorGUILayout.ObjectField("Crest Trigger", script.triggers.crestTrigger, typeof(Trigger), true);
            script.triggers.clampLowTrigger = (Trigger)EditorGUILayout.ObjectField("Clamp Low Trigger", script.triggers.clampLowTrigger, typeof(Trigger), true);
            script.triggers.clampHighTrigger = (Trigger)EditorGUILayout.ObjectField("Clamp High Trigger", script.triggers.clampHighTrigger, typeof(Trigger), true);
            script.triggers.timeOffsetTrigger = (Trigger)EditorGUILayout.ObjectField("Time Offset Trigger", script.triggers.timeOffsetTrigger, typeof(Trigger), true);
            EditorGUI.indentLevel--;
        }

        // "Line Display" section
        showLineDisplay = EditorGUILayout.Foldout(showLineDisplay, "Line Display");

        // Inside the Oscillator_LFOEditor class

        if (showLineDisplay)
        {
            EditorGUI.indentLevel++;
            script.showLine = EditorGUILayout.Toggle("Show Line", script.showLine);

            if (script.showLine)
            {
                // Check if the LineRenderer exists, and if not, provide an option to create it
                // if (script.line == null)
                // {
                //     if (GUILayout.Button("Create Line Renderer"))
                //     {
                //         script.line = script.gameObject.AddComponent<LineRenderer>();
                //         // Set default properties for the LineRenderer here
                //     }
                // }

                // If the LineRenderer exists, display options to modify it
                if (script.line != null)
                {
                    script.line.enabled = true; // Ensure the LineRenderer is enabled
                    script.lineLength = EditorGUILayout.IntField("Line Length", script.lineLength);
                    // Add more properties to modify if necessary
                }
            }
            // else
            // {
            //     // Disable the LineRenderer when 'Show Line' is unchecked
            //     if (script.line != null)
            //     {
            //         script.line.enabled = false;
            //     }
            // }

            EditorGUI.indentLevel--;
        }


        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }

    }
}
