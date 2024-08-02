using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using Transform3DBestFit;
using UnityEditor.Rendering;
using UnityEngine;

public class AnchorQTMTransform : MonoBehaviour
{
    [Header("Controls")]
    [InspectorButton("SaveQTMNames", ButtonWidth = 200)]
    public bool saveQTMNamesToScriptable = false;
    public bool hasValidData = false;
    public bool shouldSpawnPrefabs = false;

    [Header("Prefabs")]
    public GameObject AnchorPrefab;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Lists")]
    public List<string> anchorQTMNames;
    public List<Transform> anchorQTMTransforms;
    public Vector3[] vectorOfAnchorPositions;
    public Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;

    public void SaveQTMNames()
    {
        scriptableData.anchorQTMNames = anchorQTMNames;
    }

    public bool LoadFromScriptable()
    {
        hasValidData = false;

        anchorQTMNames = scriptableData.anchorQTMNames;
        anchorQTMTransforms.Clear();

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        if (shouldSpawnPrefabs)
        {
            foreach (var markerName in anchorQTMNames)
            {
                GameObject newMarker = Instantiate(AnchorPrefab, Vector3.zero, Quaternion.identity);
                newMarker.name = $"QTM Marker: {markerName}";
                newMarker.transform.parent = gameObject.transform;
                RTMarker newRTMarker = newMarker.GetComponent<RTMarker>();
                newRTMarker.MarkerName = markerName;
                anchorQTMTransforms.Add(newMarker.transform);
            }
        }

        UpdateArrays();
        hasValidData = true;
        return hasValidData;
    }
    public double[,] UpdateArrays()
    {
        vectorOfAnchorPositions = new Vector3[anchorQTMTransforms.Count];
        vector3sBackConvert = new Vector3[anchorQTMTransforms.Count];

        for (int i = 0;  i < anchorQTMTransforms.Count; i++)
        {
            vectorOfAnchorPositions[i] = anchorQTMTransforms[i].position;
        }

        pointsAsArray = Transform3D.ConvertVector3sToArray(vectorOfAnchorPositions);
        vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
        return pointsAsArray;
    }
}
