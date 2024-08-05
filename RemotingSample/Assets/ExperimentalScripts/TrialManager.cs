using System.Collections;
using TMPro;
using UnityEngine;
using Vojta.Experiment;

public class TrialManager : MonoBehaviour
{
    [Header("Controls")]

    [Header("Current Data")]
    public bool isTrialRunning = false;
    public Trial currentTrial;
    public float timeAtStart = 0;
    public float timeSinceStart = 0;
    public float currentDistanceToTarget;

    [Header("Object references")]
    private GameObject brainTargetInstance;
    public GameObject BrainTargetPrefab;
    private CoilTracker coilTracker;
    private CoilTargetPoints coilTargetPoints;

    [Header("Text Displays")]
    public TextMesh HeadsetDistanceDisplay;
    public TMP_Text PCDistanceDisplay;

    void Awake()
    {
        coilTracker = FindObjectOfType<CoilTracker>();
        coilTargetPoints = FindObjectOfType<CoilTargetPoints>();

        EventManager.OnBeginTrial += OnBeginTrial;
    }

    private void Update()
    {
        if (!isTrialRunning) return;

        timeSinceStart = Time.time - timeAtStart;
        currentDistanceToTarget = coilTracker.DistanceToTarget(currentTrial.TargetPosition);

        SetTextDisplays();

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("1"))
        {
            EndTrial();
        }
    }

    private void SetTextDisplays()
    {
        var targetVector = brainTargetInstance.transform.forward;
        var coilVector = coilTracker.coilPointForwardVector;


        var angle = (Mathf.Rad2Deg*Vector3.Dot(targetVector, coilVector)).ToString("F0");
        var distance = (currentDistanceToTarget * 1000).ToString("F0");

        string textToDisplay = $"{distance} mm \t \t {angle} deg";
        HeadsetDistanceDisplay.text = textToDisplay;
        PCDistanceDisplay.text = textToDisplay;
    }

    private void OnBeginTrial(Trial trial)
    {
        StartTrial(trial);
        StartCoroutine(TrackTrial(trial));
    }

    private void StartTrial(Trial trial)
    {
        HeadsetDistanceDisplay.GetComponent<MeshRenderer>().enabled = true;

        currentTrial = trial;
        timeAtStart = Time.time;

        isTrialRunning = true;
        // Create the trial object when the trial starts
        //brainTargetInstance = Instantiate(BrainTargetPrefab, coilTargetPoints.BrainTargetTransform.TransformPoint(trial.TargetPoint), Quaternion.identity);
        brainTargetInstance = Instantiate(BrainTargetPrefab, Vector3.zero, Quaternion.identity);
        brainTargetInstance.name = "BRAIN TARGET INSTANCE";
        brainTargetInstance.transform.parent = coilTracker.BrainTargetTransform.transform;
        brainTargetInstance.transform.localPosition = trial.TargetPosition;
        brainTargetInstance.transform.localRotation = Quaternion.Euler(trial.TargetRotation);
        // Set the target point on the CoilTracker
        //coilTracker.SetTargetPoint(brainTargetInstance.transform.position);
    }

    private IEnumerator TrackTrial(Trial trial)
    {
        //trial.TrackingStartTime = Time.time;
        //while (!trial.HasResult)
        //{
        //    // Calculate the final distance
        //    trial.FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, brainTargetInstance.transform.position);
        //    yield return null;
        //}
        //EndTrial(trial);
        //EventManager.EndTrial(trial);
        yield return null;
    }

    private void EndTrial()
    {
        HeadsetDistanceDisplay.GetComponent<MeshRenderer>().enabled = false;

        currentTrial.Duration = timeSinceStart;
        currentTrial.FinalDistance = currentDistanceToTarget;
        //// Destroy the trial object after the trial ends
        if (brainTargetInstance != null)
        {
            Destroy(brainTargetInstance);
        }

        currentTrial.HasResult = true;
        EventManager.EndTrial(currentTrial);
    }

    void OnDestroy()
    {
        EventManager.OnBeginTrial -= OnBeginTrial;
    }
}
