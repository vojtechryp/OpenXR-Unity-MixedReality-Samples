using System.Collections;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ExperimentController : MonoBehaviour
{
    public string InputParticipantId;
    public Session session;
    public bool waitingForEndOfTrial;
    public bool resultOfCurrentTrial;
    private float trialStartTime;
    public CoilTargetPoints coilTargetPoints;
    public CoilTracker coilTracker;
    public TextMeshProUGUI blockMessageText;  // Change to TextMeshProUGUI

    void Awake()
    {
        EventManager.OnEndTrial += EndOfTrial;
    }

    void Start()
    {
        coilTargetPoints = FindObjectOfType<CoilTargetPoints>();
        coilTracker = FindObjectOfType<CoilTracker>();

        // Ensure that blockMessageText is assigned
        if (blockMessageText == null)
        {
            Debug.LogError("BlockMessageText is not assigned in the Inspector");
            return;
        }

        session = new Session(InputParticipantId, coilTargetPoints, coilTracker);
        Debug.Log($"New session has been created with Id {session.ParticipantId}");

        blockMessageText.gameObject.SetActive(false);  // Hide the message text initially

        StartCoroutine(ExperimentSequence(session));
    }

    IEnumerator ExperimentSequence(Session session)
    {
        Debug.Log($"New ExperimentSequence started with Id {session.ParticipantId}");

        for (int blockNumber = 0; blockNumber < session.NumberOfBlocksPerSession; blockNumber++)
        {
            // Display message before starting the block
            if (blockNumber == 0)
            {
                ShowMessage($"Welcome, {session.ParticipantId}, to begin trial press the space button on the keyboard");
            }
            else
            {
                ShowMessage($"Block {blockNumber} finished. Take a break, and when ready press the space button on the keyboard to start Block {blockNumber + 1}");
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            HideMessage();

            Block thisBlock = session.Blocks[blockNumber];

            for (int trialNumber = 0; trialNumber < thisBlock.NumberOfTrialsInBlock; trialNumber++)
            {
                Trial thisTrial = thisBlock.Trials[trialNumber];
                thisTrial.StartTrial();
                Debug.Log($"Running trial {trialNumber + 1}, in block {blockNumber + 1}");

                // Start trial
                waitingForEndOfTrial = true;
                trialStartTime = Time.time;
                EventManager.BeginTrial(thisTrial);

                yield return new WaitWhile(() => waitingForEndOfTrial);

                thisTrial.TrialResult = resultOfCurrentTrial;
                thisTrial.FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, thisTrial.TargetPoint);
                thisTrial.Duration = Time.time - trialStartTime;
            }

            // Display break message after completing the block
            if (blockNumber < session.NumberOfBlocksPerSession - 1)
            {
                ShowMessage($"Block {blockNumber + 1} finished. Take a break, and when ready press the space button on the keyboard to start Block {blockNumber + 2}");
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                HideMessage();
            }
        }

        // Display final completion message
        ShowMessage("Experiment Completed. Thank you for participating!");
        Debug.Log("Experiment Completed");
        yield return null;
    }

    private void ShowMessage(string message)
    {
        if (blockMessageText != null)
        {
            blockMessageText.text = message;
            blockMessageText.gameObject.SetActive(true);
        }
    }

    private void HideMessage()
    {
        if (blockMessageText != null)
        {
            blockMessageText.gameObject.SetActive(false);
        }
    }

    public void EndOfTrial(bool trialResult)
    {
        resultOfCurrentTrial = trialResult;
        waitingForEndOfTrial = false;
    }

    void OnDestroy()
    {
        EventManager.OnEndTrial -= EndOfTrial;
    }
}
