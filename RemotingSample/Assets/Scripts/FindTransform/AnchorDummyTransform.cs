using Microsoft.MixedReality.OpenXR.Sample;
using System.Collections;
using System.Collections.Generic;
using Transform3DBestFit;
using Unity.XR.CoreUtils;
using UnityEngine;

public class AnchorDummyTransform : MonoBehaviour
{
    [Header("Controls")]
    public bool hasValidData = false;
    public bool shouldSpawnPrefabs = false;

    [Header("Prefabs")]
    public GameObject AnchorPrefab;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Lists")]
    public Vector3[] vectorOfAnchorPositions;
    public Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;

    public bool LoadFromScriptable()
    {
        hasValidData = false;

        vectorOfAnchorPositions = scriptableData.vectorOfAnchorPositions;

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        if (shouldSpawnPrefabs)
        {
            int i = 0;
            foreach (var anchorPosition in vectorOfAnchorPositions)
            {
                GameObject newObject = Instantiate(AnchorPrefab, anchorPosition, Quaternion.identity);
                newObject.name = $"Dummy Marker: {scriptableData.anchorNames[i]} - {scriptableData.anchorQTMNames[i]}";
                newObject.transform.parent = gameObject.transform;
            }
        }

        hasValidData = true;
        return hasValidData;
    }
    public double[,] UpdateArrays()
    {
        pointsAsArray = Transform3D.ConvertVector3sToArray(vectorOfAnchorPositions);
        vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
        return pointsAsArray;
    }
}
