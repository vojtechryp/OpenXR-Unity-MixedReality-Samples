using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Session
{
    public string ParticipantId;
    public int NumberOfBlocksPerSession = 7;
    public Block[] Blocks;
    public List<Trial> TrialResults = new List<Trial>();

    public Session(string inputParticipantId)
    {
        ParticipantId = inputParticipantId;
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


[System.Serializable]
public class Block
{
    public int NumberOfTrialsInBlock = 10;
    public Trial[] Trials;

    public Block()
    {
        Trials = new Trial[NumberOfTrialsInBlock];
        List<CoilTargetPoints.PredefinedPointStruct> randomizedPoints = CoilTargetPoints.GetRandomizedPoints();

        for (int i = 0; i < NumberOfTrialsInBlock; i++)
        {
            var pointStruct = randomizedPoints[Random.Range(0, randomizedPoints.Count)];
            Trials[i] = new Trial(i + 1, pointStruct.point, pointStruct.tag);
            randomizedPoints.Remove(pointStruct);
        }
    }
}

