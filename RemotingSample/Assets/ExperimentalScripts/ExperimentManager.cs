using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class ExperimentManager : MonoBehaviour, IMixedRealitySpeechHandler
{
    public static ExperimentManager Instance;
    public string InputParticipantId;
    public Session session;
    public TextMeshProUGUI blockMessageText;
    public string displayTypeOrder = "ARFirst";
    private CameraSetup cameraSetup;
    private bool isARCondition;
    private bool waitingForEndOfTrial;
    public GameObject BrainTarget;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        EventManager.OnEndTrial += OnEndTrial;
    }

    void Start()
    {
        // Register this script to receive input events
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        cameraSetup = FindObjectOfType<CameraSetup>();

        if (blockMessageText == null)
        {
            Debug.LogError("BlockMessageText is not assigned in the Inspector");
            return;
        }

        if (BrainTarget == null)
        {
            Debug.LogError("BrainTarget is not assigned in the Inspector");
            return;
        }

        session = new Session(InputParticipantId);
        Debug.Log($"New session has been created with Id {session.ParticipantId}");

        blockMessageText.gameObject.SetActive(false);

        isARCondition = displayTypeOrder == "ARFirst";
        StartCoroutine(ExperimentSequence(session));
    }
    
IEnumerator ExperimentSequence(Session session)
    {
        Debug.Log($"New ExperimentSequence started with Id {session.ParticipantId}");

        // Show welcome message
        ShowMessage($"Welcome, {session.ParticipantId}, to begin trial press the space button on the keyboard or say 'Next' to proceed.");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        HideMessage();

        // Loop through all blocks
        for (int blockNumber = 0; blockNumber < session.NumberOfBlocksPerSession; blockNumber++)
        {
            // Set the current condition
            SetCondition(isARCondition);

            Block thisBlock = session.Blocks[blockNumber];

            // Loop through all trials in the block
            for (int trialNumber = 0; trialNumber < thisBlock.NumberOfTrialsInBlock; trialNumber++)
            {
                Trial thisTrial = thisBlock.Trials[trialNumber];
                thisTrial.BlockNumber = blockNumber + 1;

                // Begin the trial
                EventManager.BeginTrial(thisTrial);
                waitingForEndOfTrial = true;

                // Show message indicating the trial has started
                ShowMessage("Trial started. Say 'Next' to end the trial.");
                yield return new WaitUntil(() => !waitingForEndOfTrial);
                HideMessage();

                // Add the trial result to the session
                string currentCondition = isARCondition ? "AR" : "PC";
                session.AddTrialResult(thisTrial, blockNumber + 1, displayTypeOrder, currentCondition);

                // If more trials are left in the block, wait for "Next" to proceed
                if (trialNumber < thisBlock.NumberOfTrialsInBlock - 1)
                {
                    ShowMessage("Trial complete. Say 'Next' to proceed.");
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                    HideMessage();
                }
            }

            // Save results after each block is completed
            SaveResultsToJson();

            // If more blocks are left, show message to take a break
            if (blockNumber < session.NumberOfBlocksPerSession - 1)
            {
                ShowMessage($"Block {blockNumber + 1} finished. Take a break, and when ready press the space button on the keyboard or say 'Next' to start Block {blockNumber + 2}");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                HideMessage();
            }

            // Toggle condition for the next block
            isARCondition = !isARCondition;
        }

        // Show completion message at the end of the experiment
        ShowMessage("Experiment Completed. Thank you for participating!");
        Debug.Log("Experiment Completed");
    }

    private void SetCondition(bool isAR)
    {
        // Enable AR or PC condition based on the flag
        if (isAR)
        {
            cameraSetup.SetARCondition();
        }
        else
        {
            cameraSetup.SetPCCondition();
        }
    }

    private void ShowMessage(string message)
    {
        // Display the provided message
        if (blockMessageText != null)
        {
            blockMessageText.text = message;
            blockMessageText.gameObject.SetActive(true);
        }
    }

    private void HideMessage()
    {
        // Hide the message text
        if (blockMessageText != null)
        {
            blockMessageText.gameObject.SetActive(false);
        }
    }

    public void OnEndTrial(Trial trial)
    {
        // Set the flag to indicate the trial has ended
        waitingForEndOfTrial = false;
    }

    private void SaveResultsToJson()
    {
        // Save the session results to a JSON file
        JSONManager.SaveSessionToJson(InputParticipantId, session);
        Debug.Log("Results saved successfully.");
    }

    // Implementing the speech handler interface
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        // Check if the recognized keyword is "next" and end the trial
        if (eventData.Command.Keyword.ToLower() == "next")
        {
            EndTrialByVoiceCommand();
        }
    }

    public void EndTrialByVoiceCommand()
    {
        // Set the flag to indicate the trial has ended
        waitingForEndOfTrial = false;
    }
}