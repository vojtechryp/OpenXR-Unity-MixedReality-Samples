using System;
using UnityEngine;

#if DONT_COMPILE
[System.Serializable]
public class Trial : IEquatable<Trial>
{
    public int TrialNumber;
    public Vector3 TargetPoint;
    public float FinalDistance;
    public float Duration;
    public bool TrialResult;
    public int BlockNumber;
    public string DisplayTypeOrder;
    public string CurrentCondition;
    public float TrackingStartTime;
    private string brainPositionTag;

    public Trial(int trialNumber, Vector3 targetPoint, string brainPositionTag)
    {
        TrialNumber = trialNumber;
        TargetPoint = targetPoint;
        FinalDistance = 0f;
        Duration = 0f;
        TrialResult = false;
        this.brainPositionTag = brainPositionTag;
    }

    public bool Equals(Trial otherTrial)
    {
        return TrialNumber == otherTrial.TrialNumber;
    }

    public override int GetHashCode()
    {
        return TrialNumber.GetHashCode();
    }

    public override string ToString()
    {
        return $"{TrialNumber}\t{TargetPoint}\t{FinalDistance}\t{Duration}";
    }
}
#endif