using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveableScriptable", menuName = "SaveableScriptable", order = 0)]
public class SaveableScriptable : ScriptableObject {
    
    public string FolderName = "Scriptables";
    public string FileName = "SaveableScriptable";

    public string NamePath { get => $"{Application.dataPath}/{FolderName}/{FileName}";}
    // public string AssetPath { get => $"{NamePath}.asset";}
    public string JsonPath { get => $"{NamePath}.json";}
    // public TextAsset JsonAsset { get => AssetDatabase.LoadAssetAtPath<TextAsset>(AssetPath);}

    public string SaveJson()
    {
        Debug.Log($"Saving to {JsonPath}");
        UpdateSerializeables();
        string jsonString = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(JsonPath, jsonString);
        return JsonPath;
    }

    // public string SaveJsonAsTextAsset()
    // {
    //     string jsonString = JsonUtility.ToJson(this);
    //     Debug.Log($"Saving to {AssetPath}");
        
    //     TextAsset newTextAsset = new(jsonString);
    //     if (File.Exists(AssetPath))
    //     {
    //         TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetPath);
    //         asset = newTextAsset;
    //         AssetDatabase.SaveAssetIfDirty(asset);
    //     } 
    //     else
    //     {
    //         AssetDatabase.CreateAsset(newTextAsset, AssetPath);
    //     }
    //     return AssetPath;
    // }

    public virtual void UpdateSerializeables()
    {

    }

    public void OverwriteFromJsonAsset(TextAsset jsonFileAsset)
    {
        if (jsonFileAsset != null)
        {
            JsonUtility.FromJsonOverwrite(jsonFileAsset.text, this);
        }
    }

    public void OverwriteFromJsonPath(string jsonFilePath)
    {
        TextAsset textAsset = new(File.ReadAllText(jsonFilePath));
        OverwriteFromJsonAsset(textAsset);
    }
}
