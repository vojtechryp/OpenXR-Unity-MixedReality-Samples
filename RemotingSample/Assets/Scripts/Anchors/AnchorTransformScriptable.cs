using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRLab.QTMTracking;

[CreateAssetMenu(fileName = "AnchorTransformScriptable", menuName = "AnchorTransformScriptable", order = 0)]
public class AnchorTransformScriptable : ScriptableObject {
    [SerializeField]
    public List<AnchorTransformCorrespondingPoint> correspondingPoints;
    [SerializeField]
    public List<UnityTransformMarkers> UnityMarkerNames;
    [SerializeField]
    public List<string> anchorStoredNames;
    // [SerializeField]
    private List<string> anchorQTMNames;
    [SerializeField]
    public Vector3[] vectorOfAnchorPositions;

    void OnValidate()
    {
        List<string> newanchorQTMNames = new(); 
        foreach ((int i, var correspondingPoint) in correspondingPoints.Select((value, i) => (i, value)))
        {
            UnityTransformMarkers unityMarker = correspondingPoint.UnityMarkerName;
            newanchorQTMNames.Add(QTMStaticData.GetQtmMarkerName(unityMarker));
        }
        anchorQTMNames = newanchorQTMNames;
    }

    [Serializable]
    public class AnchorTransformCorrespondingPoint
    {
        [SerializeField]
         public UnityTransformMarkers UnityMarkerName;
        [SerializeField]
        public string anchorStoredName;
        [SerializeField]
        public string anchorQTMName;
        [SerializeField]
        public Vector3 vectorOfAnchorPosition;
    }
}
