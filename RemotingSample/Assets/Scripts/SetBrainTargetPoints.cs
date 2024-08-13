using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Vojta.Experiment;

public class SetBrainTargetPoints : MonoBehaviour
{
    [InspectorButton("SaveToLocal", ButtonWidth = 200f)]
    public bool SaveToLocalButton = false;

    [InspectorButton("SaveNamesOfObjects", ButtonWidth = 200f)]
    public bool SaveNamesOfObjectsButton = false;

    [InspectorButton("SetPositionFromScriptable", ButtonWidth = 200f)]
    public bool SetPositionFromScriptableButton = false;

    [InspectorButton("SaveToScriptable", ButtonWidth = 200f)]
    public bool SaveToScriptableButton = false;

    public CoilTargetsScriptable coilTargetsScriptable;

    public List<Transform> targets = new List<Transform>();
    public List<PredefinedPointStruct> targetPoints = new List<PredefinedPointStruct>();

    public void SaveToScriptable()
    {
        coilTargetsScriptable.predefinedPoints = new(targetPoints);
    }
    public void SaveNamesOfObjects()
    {
        foreach ((int i, Transform t) in targets.Select((value, i) => (i, value)))
        {
            if (t == null) continue;

            t.gameObject.name = coilTargetsScriptable.predefinedPoints[i].Tag;
        }
    }

    public void SetPositionFromScriptable()
    {
        foreach ((int i, Transform t) in targets.Select((value, i) => (i, value)))
        {
            if (t == null) continue;

            t.localPosition = coilTargetsScriptable.predefinedPoints[i].TargetPosition;
            t.localRotation = Quaternion.Euler(coilTargetsScriptable.predefinedPoints[i].TargetRotation);
        }
    }

    public void SaveToLocal()
    {
        targetPoints = new List<PredefinedPointStruct>();
        foreach ((int i, Transform t) in targets.Select((value, i) => (i, value))) 
        {
            if (t == null) continue;

            PredefinedPointStruct thisPoint = new PredefinedPointStruct(
                t.localPosition,
                t.localEulerAngles,
                coilTargetsScriptable.predefinedPoints[i].Tag);

            targetPoints.Add(thisPoint);
        }


        //    foreach (var transform in ListOfTransforms)
        //    {
        //        if (transform == null)
        //        {
        //            return;
        //        }
        //    }

        //    predefinedPointsBackup.Add(new(predefinedPoints));

        //    predefinedPoints = new List<PredefinedPointStruct>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        var transform = ListOfTransforms[i];
        //        var tag = predefinedPointTags[i];
        //        var localForwardVector = BrainOriginTransform.transform.InverseTransformDirection(transform.forward); 
        //        predefinedPoints.Add(new PredefinedPointStruct(transform.localPosition, transform.localEulerAngles, tag));
        //    }

    }
}
