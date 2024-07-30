using System.Collections.Generic;
using UnityEngine;

public class Session
{
    public string ParticipantId;
    public int NumberOfBlocksPerSession = 4;
    public Block[] Blocks;

    public Session(string inputParticipantId, CoilTargetPoints targetPoints, CoilTracker tracker)
    {
        ParticipantId = inputParticipantId;
        Blocks = new Block[NumberOfBlocksPerSession];

        for (int i = 0; i < NumberOfBlocksPerSession; i++)
        {
            Blocks[i] = new Block(targetPoints, tracker);
        }
    }
}

public class Block
{
    public int NumberOfTrialsInBlock = 5;
    public Trial[] Trials;

    public Block(CoilTargetPoints targetPoints, CoilTracker tracker)
    {
        Trials = new Trial[NumberOfTrialsInBlock];

        // Get a new set of randomized points for each block
        List<Vector3> randomizedPoints = targetPoints.GetRandomizedPoints();

        for (int i = 0; i < NumberOfTrialsInBlock; i++)
        {
            Trials[i] = new Trial(i + 1, randomizedPoints[i], tracker, targetPoints);
        }
    }
}
