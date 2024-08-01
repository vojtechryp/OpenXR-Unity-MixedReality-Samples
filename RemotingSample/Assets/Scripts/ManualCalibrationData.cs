using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManualCalibrationData", menuName = "TransformData/ManualCalibrationData", order = 1)]
public class ManualCalibrationData : SaveableScriptable
{
    //public Transform offsetTransform;
    [SerializeField]
    public Pose offsetTransform;

}
