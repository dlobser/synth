using UnityEngine;

public class NoteToFrequency : MonoBehaviour
{
    // Example usage
    void Start()
    {
        string note = "A4";
        float frequency = NoteToFrequencyConverter(note);
        Debug.Log($"The frequency of {note} is {frequency} Hz.");
    }

    public float NoteToFrequencyConverter(string noteString)
    {
        // Parse the note string and calculate the frequency
        int noteIndex = GetNoteIndex(noteString);
        int octave = int.Parse(noteString.Substring(noteString.Length - 1));
        int midiNumber = noteIndex + (octave + 1) * 12;

        return 440f * Mathf.Pow(2f, (midiNumber - 69) / 12f);
    }

    private int GetNoteIndex(string noteString)
    {
        // Extract the note part (excluding the octave number)
        string note = noteString.Substring(0, noteString.Length - 1).ToUpper();

        switch (note)
        {
            case "C": return 0;
            case "C#": case "DB": return 1;
            case "D": return 2;
            case "D#": case "EB": return 3;
            case "E": return 4;
            case "F": return 5;
            case "F#": case "GB": return 6;
            case "G": return 7;
            case "G#": case "AB": return 8;
            case "A": return 9;
            case "A#": case "BB": return 10;
            case "B": return 11;
            default: Debug.LogError("Invalid note string"); return 0;
        }
    }
}
