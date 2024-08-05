using System;
using UnityEngine;

#if DONT_USE_THIS
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
    private CoilTracker coilTracker;
    private CoilTargetPoints coilTargetPoints;
    private GameObject visualizationSphere;
    private string brainPositionTag;

    public Trial(int trialNumber, Vector3 targetPoint, CoilTracker tracker, CoilTargetPoints targetPoints, string brainPositionTag)
    {
        TrialNumber = trialNumber;
        TargetPoint = targetPoint;
        coilTracker = tracker;
        coilTargetPoints = targetPoints;
        FinalDistance = 0f;
        Duration = 0f;
        TrialResult = false;
        this.brainPositionTag = brainPositionTag;
    }

    public void StartTrial()
    {
        visualizationSphere = coilTargetPoints.CreateVisualizationSphere(TargetPoint);
        coilTracker.SetTargetPoint(visualizationSphere.transform.position);
    }

    public void EndTrial()
    {
        FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, visualizationSphere.transform.position);
        Duration = Time.time - coilTracker.TrackingStartTime;
        TrialResult = true;
        EventManager.EndTrial(TrialResult);
        GameObject.Destroy(visualizationSphere);
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