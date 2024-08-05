using UnityEngine;
using System;

public class CoilTracker : MonoBehaviour
{
    public Transform BrainTargetTransform;
    public Transform TargetPointOnCoilTip;  // The specific point on the coil we want to track
    public Transform TargetPointOnCoilBase;  // The specific point on the coil we want to track

    private Vector3 targetPoint;
    public float TrackingStartTime;
    public event Action OnTargetReached;
    private float distanceThreshold; // Removed public access, will be set through a method
    private Vector3 relativePositionOfCoil { get => BrainTargetTransform.InverseTransformPoint(TargetPointOnCoilTip.position); }
    public void SetTargetPoint(Vector3 point)
    {
        targetPoint = point;
        TrackingStartTime = Time.time;
    }

    //public Vector3 getRelativeLocation()
    //{
    //    return BrainTargetTransform.InverseTransformPoint(TargetPointOnCoilTip.position);
    //}

    public Vector3 coilPointForwardVector
    {
        get => -TargetPointOnCoilTip.forward; 
    }


    public float DistanceToTarget(Vector3 target)
    {
        return Vector3.Distance(relativePositionOfCoil, target);
    }
    public void SetDistanceThreshold(float threshold)
    {
        distanceThreshold = threshold;
    }

    void Update()
    {
        if (targetPoint != Vector3.zero)
        {
            // Calculate the distance from the target point on the coil to the target point in the scene
            float distance = Vector3.Distance(TargetPointOnCoilTip.position, targetPoint);
            Debug.Log($"Distance to target: {distance}");
            if (distance <= distanceThreshold) // Use the adjustable threshold
            {
                OnTargetReached?.Invoke();
            }
        }
    }
}
