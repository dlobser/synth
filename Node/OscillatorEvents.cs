using System;

public static class OscillatorEvents
{
    public static event Action OnResetCounters;
    public static event Action OnUpdateEvent;

    public static void ResetCounters()
    {
        OnResetCounters?.Invoke();
    }

    public static void UpdateEvent()
    {
        OnUpdateEvent?.Invoke();
    }
}
