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
    public Session session; // Reference to the session object

    void Awake()
    {
        coilTracker = FindObjectOfType<CoilTracker>();
        coilTargetPoints = FindObjectOfType<CoilTargetPoints>();

        EventManager.OnBeginTrial += OnBeginTrial;
        VoiceCommandManager.OnNextCommand += EndTrial;
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
            UpdateDistanceTextColor(currentDistanceToTarget); // Update the color of the text based on the distance
        }

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown("2"))
        {
            EndTrial();
        }

        // Make the HeadsetDistanceDisplay always face the camera
        HeadsetDistanceDisplay.transform.rotation = Quaternion.LookRotation(HeadsetDistanceDisplay.transform.position - Camera.main.transform.position);

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
        // Ensure previous target instance is destroyed before creating a new one
        if (brainTargetInstance != null)
        {
            Destroy(brainTargetInstance);
        }

        // Create the trial object when the trial starts
        brainTargetInstance = Instantiate(BrainTargetPrefab, Vector3.zero, Quaternion.identity);
        brainTargetInstance.name = "BRAIN TARGET INSTANCE";
        brainTargetInstance.transform.parent = coilTracker.BrainTargetTransform.transform;
        brainTargetInstance.transform.localPosition = trial.TargetPosition;
        brainTargetInstance.transform.localRotation = Quaternion.Euler(trial.TargetRotation);

    }

    public void EndTrial()
    {
        HeadsetDistanceDisplay.GetComponent<MeshRenderer>().enabled = false;
        PCDistanceDisplay.text = "";

        currentTrial.Duration = timeSinceStart;
        currentTrial.FinalDistance = currentDistanceToTarget;

        // Record final positions and rotations
        currentTrial.FinalCoilPosition = coilTracker.TargetPointOnCoilTip.position;
        currentTrial.FinalCoilRotation = coilTracker.TargetPointOnCoilTip.rotation.eulerAngles;
        currentTrial.FinalBrainTargetPosition = brainTargetInstance.transform.position;
        currentTrial.FinalBrainTargetRotation = brainTargetInstance.transform.rotation.eulerAngles;
        currentTrial.FinalAngleDiscrepancy = currentAngleToTarget;

        // Destroy the trial object after the trial ends
        if (brainTargetInstance != null)
        {
            Destroy(brainTargetInstance);
            brainTargetInstance = null;  // Ensure reference is cleared
        }

        currentTrial.HasResult = true;
        isTrialRunning = false;  // Ensure trial running state is updated
        EventManager.EndTrial(currentTrial);

        // Save the session data after each trial
        SaveSessionData();
    }

    private void UpdateDistanceTextColor(float distance)
    {
        if (distance < 3.99f / 1000.0f) // 5 mm converted to meters
        {
            HeadsetDistanceDisplay.color = Color.green;
            PCDistanceDisplay.color = Color.green;
        }
        else
        {
            HeadsetDistanceDisplay.color = Color.red; // Default color, change as needed
            PCDistanceDisplay.color = Color.red; // Default color, change as needed
        }
    }

    private void SaveSessionData()
    {
        if (session != null)
        {
            session.Save();
        }
        else
        {
            Debug.LogWarning("Session reference is null. Unable to save session data.");
        }
    }

    void OnDestroy()
    {
        EventManager.OnBeginTrial -= OnBeginTrial;
        VoiceCommandManager.OnNextCommand -= EndTrial;
    }
}
