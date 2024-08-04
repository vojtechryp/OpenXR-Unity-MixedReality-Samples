using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<Trial> OnBeginTrial;
    public static event Action<Trial> OnEndTrial;

    public static void BeginTrial(Trial trial)
    {
        OnBeginTrial?.Invoke(trial);
    }

    public static void EndTrial(Trial trial)
    {
        OnEndTrial?.Invoke(trial);
    }
}
