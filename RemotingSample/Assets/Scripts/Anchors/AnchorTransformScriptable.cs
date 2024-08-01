using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "AnchorTransformScriptable", menuName = "AnchorTransformScriptable", order = 0)]
public class AnchorTransformScriptable : ScriptableObject {
    [SerializeField]
    public List<string> anchorNames = null;
    [SerializeField]
    public List<string> anchorQTMNames = null;
    [SerializeField]
    public Vector3[] vectorOfAnchorPositions = null;
}
