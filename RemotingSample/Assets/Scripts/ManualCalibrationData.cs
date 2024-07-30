using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManualCalibrationData", menuName = "TransformData/ManualCalibrationData", order = 1)]
public class ManualCalibrationData : ScriptableObject
{
    //public Transform offsetTransform;
    [SerializeField]
    public Pose offsetTransform;

    public string SaveJson()
    {
        Debug.Log($"Saving to {Application.dataPath}");
        string jsonString = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(Application.dataPath + "/ManualCalibrationData.json", jsonString);
        return Application.dataPath + "/ManualCalibrationData.json";
    }

    public static void OverwriteFromJson(ManualCalibrationData transformData, TextAsset jsonFile)
    {
        JsonUtility.FromJsonOverwrite(jsonFile.text, transformData);
    }
}
