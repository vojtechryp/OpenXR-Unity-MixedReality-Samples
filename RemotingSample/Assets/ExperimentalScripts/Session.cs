using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
public enum FirstCondition
{
    ARFirst,
    PCFirst
}

public enum DisplayCondition
{
    AR,
    PC
}

namespace Vojta.Experiment
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Session", menuName = "Experiment/Session Scriptable", order = 0)]
    public class Session : ScriptableObject
    {
        [SerializeField]
        public string ParticipantId;
        [SerializeField]
        public FirstCondition FirstCondition;
        [SerializeField]
        public int NumberOfBlocksPerSession = 7;
        [SerializeField]
        public Block[] ARBlocks;
        [SerializeField]
        public Block[] PCBlocks;
        [SerializeField]
        public List<Trial> TrialResults = new List<Trial>();
        public string FileName { get { return getFileName(ParticipantId); } }
        private const string relativeAssetPath = "Assets/Results/Session/";
        private const string relativeJsonPath = "Assets/Results/Json/";



        public static string getFileName(string participantId)
        {
            return $"{participantId}";
        }

        public static Session GetSessionAsset(string inputParticipantId, FirstCondition firstCondition)
        {
            string fullSessionFileName = getFileName(inputParticipantId);
            string fullAssetPath = $"{relativeAssetPath}/{fullSessionFileName}.asset";
            Session tryLoadSession = AssetDatabase.LoadAssetAtPath<Session>(fullAssetPath);

            if (tryLoadSession == null)
            {
                Debug.Log($"No session scriptable found for Participant {inputParticipantId}. Creating new file.");
                tryLoadSession = new Session(inputParticipantId, firstCondition);
                AssetDatabase.CreateAsset(tryLoadSession, fullAssetPath);
                tryLoadSession.Save();
            }
            else
            {
                tryLoadSession.LoadFromJson();
                Debug.Log($"Session scriptable found for Participant {inputParticipantId}.");
            }
            return tryLoadSession;
        }
        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            JsonUtility.ToJson(this);
            string fullJsonFilenameAndPath = $"{relativeJsonPath}/{FileName}.json";
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

        public Session(string inputParticipantId, FirstCondition firstCondition)
        {
            Initialize(inputParticipantId, firstCondition);
        }

        private void Initialize(string inputParticipantId, FirstCondition firstCondition)
        {
            Random.InitState((int)DateTime.Now.Ticks);
            ParticipantId = inputParticipantId;
            FirstCondition = firstCondition;
            ARBlocks = new Block[NumberOfBlocksPerSession];
            PCBlocks = new Block[NumberOfBlocksPerSession];

            for (int i = 0; i < NumberOfBlocksPerSession; i++)
            {
                ARBlocks[i] = new Block(FirstCondition, DisplayCondition.AR, i);
                PCBlocks[i] = new Block(FirstCondition, DisplayCondition.PC, i);
            }
        }
    }


    [System.Serializable]
    public class Block
    {
        public int NumberOfTrialsInBlock = 10;
        public int BlockNumber;
        [SerializeField]
        public Trial[] Trials;
        [SerializeField]
        public FirstCondition FirstCondition;
        [SerializeField]
        public DisplayCondition CurrentCondition;
        public bool IsBlockComplete
        {
            get
            {
                foreach (Trial trial in Trials) {
                    if (!trial.HasResult) return false;
                }
                return true;
            }
        }
        public Block(FirstCondition firstCondition, DisplayCondition displayCondition, int blockNumber)
        {
            FirstCondition = firstCondition;
            CurrentCondition = displayCondition;
            BlockNumber = blockNumber;

            Trials = new Trial[NumberOfTrialsInBlock];
            var randomizedPoints = CoilTargetPoints.GetRandomizedPoints();

            Assert.AreEqual(randomizedPoints.Count, NumberOfTrialsInBlock);

            for (int i = 0; i < NumberOfTrialsInBlock; i++)
            {
                var pointStruct = randomizedPoints[i];
                Trials[i] = new Trial(i, BlockNumber, pointStruct.TargetPosition, pointStruct.TargetRotation, pointStruct.Tag, FirstCondition, CurrentCondition);
            }
        }
    }

    [Serializable]
    public class Trial : IEquatable<Trial>
    {
        [Header("Basic Info")]
        public int TrialNumber;
        public int BlockNumber;

        [Header("Conditions")]
        public Vector3 TargetPosition;
        public Vector3 TargetRotation;
        private string BrainPositionTag;
        public FirstCondition FirstCondition;
        public DisplayCondition CurrentCondition;

        [Header("Results")]
        public float FinalDistance = 0.0f;
        public float Duration = 0.0f;
        public Vector3 FinalCoilPosition;
        public Vector3 FinalCoilRotation;
        public bool HasResult = false;

        public Trial(int trialNumber, int blockNumber, Vector3 targetPosition, Vector3 targetRotation, string brainPositionTag, FirstCondition firstCondition, DisplayCondition currentCondition)
        {
            TrialNumber = trialNumber;
            TargetPosition = targetPosition;
            TargetRotation = targetRotation;

            BlockNumber = blockNumber;
            BrainPositionTag = brainPositionTag;
            FirstCondition = firstCondition;
            CurrentCondition = currentCondition;
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
            return $"{TrialNumber}\t{TargetPosition}\t{FinalDistance}\t{Duration}";
        }
    }
}
