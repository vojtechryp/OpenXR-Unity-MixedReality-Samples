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
    public AnchorQTMTransform qtmTransform;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Lists")]
    public Transform[] prefabTransforms;
    public Vector3[] vectorOfAnchorPositions;
    public Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;

    public bool LoadFromScriptable()
    {
        hasValidData = false;

        qtmTransform.UpdateArrays();

        vectorOfAnchorPositions = new Vector3[qtmTransform.anchorQTMTransforms.Count];
        vector3sBackConvert = new Vector3[qtmTransform.anchorQTMTransforms.Count];
        prefabTransforms = new Transform[qtmTransform.anchorQTMTransforms.Count];

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        if (shouldSpawnPrefabs)
        {
            int i = 0;
            foreach (var qtmTransform in qtmTransform.anchorQTMTransforms)
            {
                GameObject newObject = Instantiate(AnchorPrefab, qtmTransform.position, Quaternion.identity);
                newObject.name = $"Dummy Marker: {scriptableData.anchorNames[i]} - {scriptableData.anchorQTMNames[i]}";
                newObject.transform.parent = gameObject.transform;
                vectorOfAnchorPositions[i] = qtmTransform.position;
                prefabTransforms[i] = gameObject.transform;

                i++;
            }
        }

        UpdateArrays();
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
