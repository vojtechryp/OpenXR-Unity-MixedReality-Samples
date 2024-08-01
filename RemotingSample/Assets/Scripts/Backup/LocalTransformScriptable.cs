using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalTransformScriptable", menuName = "LocalTransformScriptable", order = 0)]
public class LocalTransformScriptable : SaveableScriptable {
    
    [SerializeField]
    public Pose thisPose;

    public LocalTransformScriptable(string objectName, Transform transform)
    {
        FolderName = "SaveableScriptables";
        FileName = objectName;
        thisPose = transform.GetLocalPose();
    }

    public void UpdateValues(Transform transform)
    {
        thisPose = transform.GetLocalPose();
    }
}
