using System.Collections;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    private CoilTracker coilTracker;
    private CoilTargetPoints coilTargetPoints;
    private GameObject trialObjectInstance;

    void Awake()
    {
        coilTracker = FindObjectOfType<CoilTracker>();
        coilTargetPoints = FindObjectOfType<CoilTargetPoints>();

        EventManager.OnBeginTrial += OnBeginTrial;
    }

    private void OnBeginTrial(Trial trial)
    {
        StartTrial(trial);
        StartCoroutine(TrackTrial(trial));
    }

    private void StartTrial(Trial trial)
    {
        // Create the trial object when the trial starts
        Vector3 targetPosition = ExperimentManager.Instance.brainTargetTransform.TransformPoint(trial.TargetPoint);
        trialObjectInstance = Instantiate(ExperimentManager.Instance.trialObjectPrefab, targetPosition, Quaternion.identity);
        // Set the target point on the CoilTracker
        coilTracker.SetTargetPoint(trialObjectInstance.transform.position);
    }

    private IEnumerator TrackTrial(Trial trial)
    {
        trial.TrackingStartTime = Time.time;
        while (!trial.TrialResult)
        {
            // Calculate the final distance
            trial.FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, trialObjectInstance.transform.position);
            yield return null;
        }
        EndTrial(trial);
        EventManager.EndTrial(trial);
    }

    private void EndTrial(Trial trial)
    {
        trial.Duration = Time.time - trial.TrackingStartTime;
        trial.TrialResult = true;
        // Destroy the trial object after the trial ends
        if (trialObjectInstance != null)
        {
            Destroy(trialObjectInstance);
        }
    }

    void OnDestroy()
    {
        EventManager.OnBeginTrial -= OnBeginTrial;
    }
}
