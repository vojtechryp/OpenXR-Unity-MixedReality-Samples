using System;
using UnityEngine;

[System.Serializable]
public class Trial : IEquatable<Trial>
{
    public int TrialNumber;
    public Vector3 TargetPoint;
    public float FinalDistance;
    public float Duration;
    public bool TrialResult;
    private CoilTracker coilTracker;
    private CoilTargetPoints coilTargetPoints;
    private GameObject visualizationSphere;

    public Trial(int trialNumber, Vector3 targetPoint, CoilTracker tracker, CoilTargetPoints targetPoints)
    {
        TrialNumber = trialNumber;
        TargetPoint = targetPoint;
        coilTracker = tracker;
        coilTargetPoints = targetPoints;
        FinalDistance = 0f;
        Duration = 0f;
        TrialResult = false;
    }

    public void StartTrial()
    {
        // Create visualization sphere when the trial starts
        visualizationSphere = coilTargetPoints.CreateVisualizationSphere(TargetPoint);

        // Set the target point on the BrainPosition GameObject
        coilTracker.SetTargetPoint(visualizationSphere.transform.position); // Use the world position of the visualization sphere
        coilTracker.OnTargetReached += EndTrial;
    }

    public void EndTrial()
    {
        // Calculate the final distance
        FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, visualizationSphere.transform.position);
        Duration = Time.time - coilTracker.TrackingStartTime;
        TrialResult = true;
        coilTracker.OnTargetReached -= EndTrial;

        // Notify the experiment controller that the trial has ended
        EventManager.EndTrial(TrialResult);

        // Destroy the visualization sphere after the trial ends
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
