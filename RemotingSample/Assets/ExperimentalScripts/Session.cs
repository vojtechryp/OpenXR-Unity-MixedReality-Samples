using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Session
{
    public string ParticipantId;
    public int NumberOfBlocksPerSession = 4;
    public Block[] Blocks;
    public List<Trial> TrialResults = new List<Trial>();

    public Session(string inputParticipantId, CoilTargetPoints targetPoints, CoilTracker tracker)
    {
        ParticipantId = inputParticipantId;
        Blocks = new Block[NumberOfBlocksPerSession];

        for (int i = 0; i < NumberOfBlocksPerSession; i++)
        {
            Blocks[i] = new Block(targetPoints, tracker);
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

[System.Serializable]
public class Block
{
    public int NumberOfTrialsInBlock = 5;
    public Trial[] Trials;

    public Block(CoilTargetPoints targetPoints, CoilTracker tracker)
    {
        Trials = new Trial[NumberOfTrialsInBlock];

        List<Vector3> randomizedPoints = CoilTargetPoints.GetRandomizedPoints();

        for (int i = 0; i < NumberOfTrialsInBlock; i++)
        {
            Vector3 point = randomizedPoints[i];
            string tag = CoilTargetPoints.GetBrainPositionTag(point);
            Trials[i] = new Trial(i + 1, point, tracker, targetPoints, tag);
        }
    }
}
