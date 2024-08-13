using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

namespace Vojta.Experiment
{
    public class ExperimentManager : MonoBehaviour, IMixedRealitySpeechHandler
    {
        [InspectorButton("CreateOrFindParticipantSession", ButtonWidth = 300)]
        public bool CreateOrFindParticipantSessionButton = false;
        public string InputParticipantId;
        public FirstCondition FirstConditionInSession = FirstCondition.ARFirst;
        public DisplayCondition ConditionToRun;
        public Session session;
        public TextMeshProUGUI blockMessageText;
        private bool waitingForEndOfTrial;
        private bool isKeyToProgressPressed { get => (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("4")); }

        [Header("Voice control")]
        public static ExperimentManager Instance;

        public void CreateOrFindParticipantSession()
        {
            session = Session.GetSessionAsset(InputParticipantId, FirstConditionInSession);

            if (session == null)
            {
                Debug.LogError("Session file not found or created correctly!");
                return;
            }
            FirstConditionInSession = session.FirstCondition;

            // Set the session reference in the TrialManager
            var trialManager = FindObjectOfType<TrialManager>();
            if (trialManager != null)
            {
                trialManager.session = session;
            }
            else
            {
                Debug.LogError("TrialManager not found in the scene.");
            }
        }

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
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);

            if (blockMessageText == null)
            {
                Debug.LogError("BlockMessageText is not assigned in the Inspector");
                return;
            }

            CreateOrFindParticipantSession();

            blockMessageText.gameObject.SetActive(false);

            StartCoroutine(ExperimentSequence(session));
        }

        IEnumerator ExperimentSequence(Session session)
        {
            Debug.Log($"New ExperimentSequence started with Id {session.ParticipantId}");

            ShowMessage($"Welcome, {session.ParticipantId}, to begin trial press the space button on the keyboard or say 'Next' to proceed.");
            yield return new WaitUntil(() => isKeyToProgressPressed);
            HideMessage();

            Block[] blocksToRun = ConditionToRun == DisplayCondition.PC ? session.PCBlocks : session.ARBlocks;

            for (int blockNumber = 0; blockNumber < session.NumberOfBlocksPerSession; blockNumber++)
            {
                Block thisBlock = blocksToRun[blockNumber];

                if (thisBlock.IsBlockComplete) continue;

                for (int trialNumber = 0; trialNumber < thisBlock.NumberOfTrialsInBlock; trialNumber++)
                {
                    Trial thisTrial = thisBlock.Trials[trialNumber];

                    if (thisTrial.HasResult) continue;

                    EventManager.BeginTrial(thisTrial);
                    waitingForEndOfTrial = true;

                    ShowMessage("Trial started. Press TMS to End trial.");
                    yield return new WaitWhile(() => waitingForEndOfTrial);
                    HideMessage();

                    yield return new WaitForSeconds(3f);
                }

                // Save results after each block is completed
                SaveResultsToJson();

                if (blockNumber < session.NumberOfBlocksPerSession - 1)
                {
                    ShowMessage($"Block {blockNumber + 1} finished. Take a break, and when ready press the Block button to start Block {blockNumber + 2}");
                    yield return new WaitUntil(() => isKeyToProgressPressed);
                    HideMessage();
                }
            }

            ShowMessage("Experiment Completed. Thank you for participating!");
            Debug.Log("Experiment Completed");
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

        public void OnEndTrial(Trial trial)
        {
            waitingForEndOfTrial = false;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.Command.Keyword.ToLower() == "next")
            {
                EndTrialByVoiceCommand();
            }
        }

        public void EndTrialByVoiceCommand()
        {
            var trialManager = FindObjectOfType<TrialManager>();
            if (trialManager != null && trialManager.isTrialRunning)
            {
                trialManager.EndTrial();
            }
            else
            {
                waitingForEndOfTrial = false;
            }
        }

        private void SaveResultsToJson()
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
    }
}
