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
    public float currentAngleToTarget;

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

        if (brainTargetInstance != null)
        {
            var targetVector = brainTargetInstance.transform.forward;
            var coilVector = coilTracker.coilPointForwardVector;

            currentDistanceToTarget = coilTracker.DistanceToTarget(currentTrial.TargetPosition);
            currentAngleToTarget = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(targetVector, coilVector));

            SetTextDisplays();
        }


        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("1"))
        {
            EndTrial();
        }
    }

    private void SetTextDisplays()
    {
        var angle = currentAngleToTarget.ToString("F0");
        var distance = (currentDistanceToTarget * 1000).ToString("F0");

        string textToDisplay = $"{distance} mm \t \t {angle} deg";
        HeadsetDistanceDisplay.text = textToDisplay;
        PCDistanceDisplay.text = textToDisplay;
    }

    private void OnBeginTrial(Trial trial)
    {
        StartTrial(trial);
    }

    private void StartTrial(Trial trial)
    {
        HeadsetDistanceDisplay.GetComponent<MeshRenderer>().enabled = true;

        currentTrial = trial;
        timeAtStart = Time.time;

        isTrialRunning = true;
        // Create the trial object when the trial starts
        brainTargetInstance = Instantiate(BrainTargetPrefab, Vector3.zero, Quaternion.identity);
        brainTargetInstance.name = "BRAIN TARGET INSTANCE";
        brainTargetInstance.transform.parent = coilTracker.BrainTargetTransform.transform;
        brainTargetInstance.transform.localPosition = trial.TargetPosition;
        brainTargetInstance.transform.localRotation = Quaternion.Euler(trial.TargetRotation);
    }

    private void EndTrial()
    {
        HeadsetDistanceDisplay.GetComponent<MeshRenderer>().enabled = false;
        PCDistanceDisplay.text = "";

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
