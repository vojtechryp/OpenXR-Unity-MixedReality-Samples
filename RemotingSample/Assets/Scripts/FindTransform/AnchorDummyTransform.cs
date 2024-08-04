using Microsoft.MixedReality.OpenXR.Sample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transform3DBestFit;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace VRLab.QTMTracking
{
public class AnchorDummyTransform : MonoBehaviour
{
    [Header("Controls")]
    [InspectorButton("ClearPrefabs", ButtonWidth = 200)]
    public bool clearChildPrefabs = false;
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

        ClearPrefabs();

        if (shouldSpawnPrefabs)
        {
            // int i = 0;
            foreach ((int i, var qtmTransform) in qtmTransform.anchorQTMTransforms.Select((value, i) => ( i, value )))
            {
                GameObject newObject = Instantiate(AnchorPrefab, qtmTransform.position, Quaternion.identity);
                newObject.name = $"Dummy Marker: {scriptableData.anchorStoredNames[i]} - {scriptableData.correspondingPoints[i].anchorQTMName}";
                newObject.transform.parent = gameObject.transform;
                vectorOfAnchorPositions[i] = qtmTransform.position;
                prefabTransforms[i] = gameObject.transform;
            }
        }

        UpdateArrays();
        hasValidData = true;
        return hasValidData;
    }

    private void ClearPrefabs()
    {
        while ( gameObject.transform.childCount>0) DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public double[,] UpdateArrays()
    {
        pointsAsArray = Transform3D.ConvertVector3sToArray(vectorOfAnchorPositions);
        vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
        return pointsAsArray;
    }
}
}