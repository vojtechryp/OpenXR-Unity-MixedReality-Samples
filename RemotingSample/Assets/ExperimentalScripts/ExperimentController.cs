using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System;

public enum DisplayTypeOrder
{
    ARFirst,
    PCFirst
}

public class ExperimentController : MonoBehaviour, IMixedRealitySpeechHandler
{
    // Instance?
    public static ExperimentController Instance;

    [Header("Inputs")]
    public string InputParticipantId;
    public DisplayTypeOrder displayTypeOrder = DisplayTypeOrder.ARFirst;
    
    [Header("Inputs")]
    public Session session;

    [Header("Status Booleans")]
    public bool waitingForEndOfTrial;
    public bool resultOfCurrentTrial;
    // private float trialStartTime;
    // public CoilTargetPoints coilTargetPoints;
    // public CoilTracker coilTracker;
    // public TextMeshProUGUI blockMessageText;

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

        EventManager.OnEndTrial += EndOfTrial;
    }

    void Start()
    {
        // Register this script to receive input events
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

        // if (coilTargetPoints == null)
        // {
        //     Debug.LogError($"COIL TARGET POINTS ARE NULL!!!!!");
        // }
        // if (coilTracker == null)
        // {
        //     Debug.LogError($"COIL TRACKER POINTS ARE NULL!!!!!");
        // }

        // if (blockMessageText == null)
        // {
        //     Debug.LogError("BlockMessageText is not assigned in the Inspector");
        //     return;
        // }

        // session = new Session(InputParticipantId, coilTargetPoints, coilTracker);
        session = Session.GetSessionAsset(InputParticipantId);
        Debug.Log($"New session has been created with Id {session.ParticipantId}");

        // TODO Move this its own functionality
        // blockMessageText.gameObject.SetActive(false);

        StartCoroutine(ExperimentSequence(session));
    }

    IEnumerator ExperimentSequence(Session session)
    {
        Debug.Log($"New ExperimentSequence started with Id {session.ParticipantId}");

        ShowMessage($"Welcome, {session.ParticipantId}, to begin trial press the space button on the keyboard or say 'Next' to proceed.");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        HideMessage();

        for (int blockNumber = 0; blockNumber < session.NumberOfBlocksPerSession; blockNumber++)
        {
            Block thisBlock = session.Blocks[blockNumber];

            for (int trialNumber = 0; trialNumber < thisBlock.NumberOfTrialsInBlock; trialNumber++)
            {
                Trial thisTrial = thisBlock.Trials[trialNumber];
                thisTrial.StartTrial(); // Starting trial should be done through BeginTrial event only ...
                Debug.Log($"Running trial {trialNumber + 1}, in block {blockNumber + 1}");

                waitingForEndOfTrial = true;

                // TODO Move this to Trial Manager OnBeginTrial
                // trialStartTime = Time.time;
                EventManager.BeginTrial(thisTrial);

                yield return new WaitWhile(() => waitingForEndOfTrial);

                // TODO Do all of this in Trial Manager
                // thisTrial.IsComplete = resultOfCurrentTrial;
                // thisTrial.FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, thisTrial.TargetPoint);
                // thisTrial.Duration = Time.time - trialStartTime;

                // string currentCondition = displayTypeOrder.Contains("AR") ? "AR" : "PC";

                //? Is this needed?
                // session.AddTrialResult(thisTrial, blockNumber + 1, displayTypeOrder, currentCondition);

                thisTrial.EndTrial(); // This will destroy the sphere

                //? Is this correct? If so, replace the yield statement with a different Event with its own "waitingFor" boolean
                // If more trials are left in the block, wait for "Next" to proceed
                if (trialNumber < thisBlock.NumberOfTrialsInBlock - 1)
                {
                    ShowMessage("Trial complete. Say 'Next' to proceed.");
                    yield return new WaitUntil(() => !waitingForEndOfTrial);
                    HideMessage();
                }
            }

            // Save results after each block is completed
            session.Save();

            if (blockNumber < session.NumberOfBlocksPerSession - 1)
            {
                ShowMessage($"Block {blockNumber + 1} finished. Take a break, and when ready press the space button on the keyboard or say 'Next' to start Block {blockNumber + 2}");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                HideMessage();
            }
        }

        ShowMessage("Experiment Completed. Thank you for participating!");
        Debug.Log("Experiment Completed");
        yield return null;
    }

    private void ShowMessage(string message)
    {
        throw new NotImplementedException();
        // TODO Move this to its own script and send as event
        // if (blockMessageText != null)
        // {
        //     blockMessageText.text = message;
        //     blockMessageText.gameObject.SetActive(true);
        // }
    }

    private void HideMessage()
    {
        throw new NotImplementedException();
        // TODO Move this to its own script and send as event
        // if (blockMessageText != null)
        // {
        //     blockMessageText.gameObject.SetActive(false);
        // }
    }

    public void EndOfTrial(bool trialResult)
    {
        resultOfCurrentTrial = trialResult;
        waitingForEndOfTrial = false;
    }

    void OnDestroy()
    {
        EventManager.OnEndTrial -= EndOfTrial;
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
    }

    private void SaveResultsToJson()
    {
        JSONManager.SaveSessionToJson(InputParticipantId, session);
        Debug.Log("Results saved successfully.");
    }

    // Implementing the speech handler interface
    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.Command.Keyword.ToLower() == "next")
        {
            EndTrialByVoiceCommand();
        }
    }

    public void EndTrialByVoiceCommand()
    {
        // Set the result of the current trial to true and end the trial
        resultOfCurrentTrial = true;
        waitingForEndOfTrial = false;
    }
}
