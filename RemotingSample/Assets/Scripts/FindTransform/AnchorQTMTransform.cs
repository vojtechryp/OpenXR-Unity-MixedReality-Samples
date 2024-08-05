using QualisysRealTime.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transform3DBestFit;
using UnityEditor.Rendering;
using UnityEngine;
using VRLab.QTMTracking;

namespace VRLab.QTMTracking
{
public class AnchorQTMTransform : MonoBehaviour
{
    [Header("Controls")]
    // [InspectorButton("SaveQTMNames", ButtonWidth = 200)]
    // public bool saveQTMNamesToScriptable = false;
    [SerializeField]
    QTMStaticDataScriptable QTMStaticDataScriptable = null;
    [InspectorButton("ClearPrefabs", ButtonWidth = 200)]
    public bool clearChildPrefabs = false;
    public bool hasValidData = false;
    public bool shouldSpawnPrefabs = false;

    [Header("Prefabs")]
    public GameObject AnchorPrefab;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Lists")]
    // public List<string> anchorQTMNames;
    public List<UnityTransformMarkers> UnityMarkers;
    public List<Transform> anchorQTMTransforms;
    public Vector3[] vectorOfAnchorPositions;
    public Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;

    public void SaveQTMNames()
    {
    //     scriptableData.anchorQTMNames = anchorQTMNames;
    }

    public bool LoadFromScriptable()
    {
        hasValidData = false;

        // anchorQTMNames = scriptableData.anchorQTMNames;
        UnityMarkers = scriptableData.UnityMarkerNames;
        anchorQTMTransforms.Clear();

        ClearPrefabs();

        if (shouldSpawnPrefabs)
        {
            // foreach (var markerName in anchorQTMNames.Select((value, i) => new { i, value }))
            // {
            //     GameObject newMarker = Instantiate(AnchorPrefab, Vector3.zero, Quaternion.identity);
            //     newMarker.name = $"QTM Marker: {markerName.value}";
            //     newMarker.transform.parent = gameObject.transform;
            //     newMarker.transform.position = scriptableData.vectorOfAnchorPositions[markerName.i];
            //     RTMarker newRTMarker = newMarker.GetComponent<RTMarker>();
            //     newRTMarker.MarkerName = markerName.value;
            //     anchorQTMTransforms.Add(newMarker.transform);
            // }

            foreach ((int i, UnityTransformMarkers markerEnum) in UnityMarkers.Select((value, i) => (i, value)))
            {
                string markerQTMName = QTMStaticDataScriptable.GetQtmMarkerName(markerEnum);
                GameObject newMarker = Instantiate(AnchorPrefab, Vector3.zero, Quaternion.identity);

                newMarker.name = $"QTM Marker: {markerQTMName}";
                newMarker.transform.parent = gameObject.transform;
                newMarker.transform.position = scriptableData.vectorOfAnchorPositions[i];

                RTMarker newRTMarker = newMarker.GetComponent<RTMarker>();
                newRTMarker.MarkerName = markerQTMName;
                anchorQTMTransforms.Add(newMarker.transform);
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
        private void Update()
        {
            for (int i = 0; i < anchorQTMTransforms.Count; i++)
            {
                if (anchorQTMTransforms[i] != null)
                { 
                    vectorOfAnchorPositions[i] = anchorQTMTransforms[i].position; 
                }
            }
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
}
