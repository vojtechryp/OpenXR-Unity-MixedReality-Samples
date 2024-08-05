using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<bool> OnEndTrial;
    public static event Action<Trial> OnBeginTrial;

    public static void EndTrial(bool trialResult)
    {
        OnEndTrial?.Invoke(trialResult);
    }

    public static void BeginTrial(Trial trial)
    {
        OnBeginTrial?.Invoke(trial);
    }
}
