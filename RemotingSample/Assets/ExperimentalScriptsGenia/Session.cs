using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Do.Not.Use
{
    [Serializable]
    [CreateAssetMenu(fileName = "Session", menuName = "Experiment/Session Scriptable", order = 0)]
    public class Session : ScriptableObject
    {
        [SerializeField]
        public string ParticipantId;
        [SerializeField]
        public int NumberOfBlocksPerSession = 4;
        [SerializeField]
        public Block[] Blocks;
        [SerializeField]
        public List<Trial> TrialResults = new List<Trial>();

        public string FileName { get { return getFileName(ParticipantId); } }
        private const string relativeAssetPath = "/Results/Session/";
        private const string relativeJsonPath = "/Results/Json/";

        public Session(string participantId)
        {
            ParticipantId = participantId;
            Initialize();
        }

        public static string getFileName(string participantId)
        {
            return $"{participantId}_Session";
        }

        public static Session GetSessionAsset(string inputParticipantId)
        {
            string fullSessionFileName = getFileName(inputParticipantId);
            string fullAssetPath = $"{relativeAssetPath}/{fullSessionFileName}.asset";
            Session tryLoadSession = AssetDatabase.LoadAssetAtPath<Session>(fullAssetPath);
            if (tryLoadSession == null)
            {
                tryLoadSession = new Session(inputParticipantId);
                AssetDatabase.CreateAsset(tryLoadSession, fullAssetPath);
                tryLoadSession.Save();
            }
            else
            {
                tryLoadSession.LoadFromJson();
            }
            return tryLoadSession;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            JsonUtility.ToJson(this);
            string fullJsonFilenameAndPath = $"{relativeJsonPath}/JSON/{FileName}.json";
            File.WriteAllText(fullJsonFilenameAndPath, JsonUtility.ToJson(this));
        }

        public void LoadFromJson()
        {
            string fullJsonFilenameAndPath = $"{relativeJsonPath}/JSON/{FileName}.json";
            if (File.Exists(fullJsonFilenameAndPath))
            {
                string jsonString = File.ReadAllText(fullJsonFilenameAndPath);
                JsonUtility.FromJsonOverwrite(jsonString, this);
            }
        }

        private void Initialize()
        {
            Blocks = new Block[NumberOfBlocksPerSession];

            for (int i = 0; i < NumberOfBlocksPerSession; i++)
            {
                Blocks[i] = new Block();
            }
        }

        public void AddTrialResult(Trial trial, int blockNumber, string displayTypeOrder, string currentCondition)
        {
            trial.BlockNumber = blockNumber;
            trial.DisplayTypeOrder = displayTypeOrder;
            trial.CurrentCondition = currentCondition;
            TrialResults.Add(trial);
        }
    }

    [Serializable]
    public class Block
    {
        public int NumberOfTrialsInBlock = 5;
        public Trial[] Trials;

        public Block()
        {
            Trials = new Trial[NumberOfTrialsInBlock];

            List<Vector3> randomizedPoints = CoilTargetPoints.GetRandomizedPoints();

            for (int i = 0; i < NumberOfTrialsInBlock; i++)
            {
                Vector3 point = randomizedPoints[i];
                string tag = CoilTargetPoints.GetBrainPositionTag(point);
                Trials[i] = new Trial(i + 1, point, tag);
            }
        }
    }

    [Serializable]
    public class Trial : IEquatable<Trial>
    {
        // Basic info
        [SerializeField]
        public int TrialNumber;
        [SerializeField]
        public int BlockNumber;

        // Conditions
        [SerializeField]
        public Vector3 TargetPoint;
        [SerializeField]
        public string DisplayTypeOrder;
        [SerializeField]
        public string CurrentCondition; // Whats this for?

        // Results
        [SerializeField]
        public float FinalDistance;
        [SerializeField]
        public Vector3 FinalCoilPosition;
        [SerializeField]
        public float Duration;
        [SerializeField]
        public bool IsComplete;
        [SerializeField]
        private string BrainPositionTag;

        public Trial(int trialNumber, Vector3 targetPoint, string brainPositionTag)
        {
            TrialNumber = trialNumber;
            TargetPoint = targetPoint;
            FinalDistance = 0f;
            Duration = 0f;
            IsComplete = false;
            BrainPositionTag = brainPositionTag;
        }

        public void SetResult(float finalDistance, float duration, Vector3 finalCoilPosition)
        {
            FinalDistance = finalDistance;
            Duration = duration;
            FinalCoilPosition = finalCoilPosition;
            IsComplete = true;
        }

        // MOVE THIS FUNCTIONALITY TO TRIAL MANAGER
        public void StartTrial()
        {
            throw new NotImplementedException();
            // visualizationSphere = coilTargetPoints.CreateVisualizationSphere(TargetPoint);
            // coilTracker.SetTargetPoint(visualizationSphere.transform.position);
        }

        // MOVE THIS FUNCTIONALITY TO TRIAL MANAGER
        public void EndTrial()
        {
            throw new NotImplementedException();
            // FinalDistance = Vector3.Distance(coilTracker.targetPointOnCoil.position, visualizationSphere.transform.position);
            // Duration = Time.time - coilTracker.TrackingStartTime;
            // TrialResult = true;
            // EventManager.EndTrial(TrialResult);
            // GameObject.Destroy(visualizationSphere);
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
            return $"{BlockNumber}\t{TrialNumber}\t{TargetPoint}\t{FinalDistance}\t{Duration}";
        }
    }
}


