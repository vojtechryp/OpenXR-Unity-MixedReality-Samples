using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

public class LocalTransformSaveable : MonoBehaviour
{
    public bool resetGuid = false;
    public LocalTransformScriptable scriptable;
    public string saveName = "InitialName";
    public bool hasBeenSaved = false;
    

    private void OnValidate() {
        if (!scriptable && saveName != "InitialName" && !hasBeenSaved)
        {
            SaveJson();
            hasBeenSaved = true;
        }        
        // UpdateFromScriptable();
    }

    // Start is called before the first frame update
    void Start()
    {
        scriptable.OverwriteFromJsonPath(scriptable.JsonPath);
        UpdateFromScriptable();
    }

    private void UpdateFromScriptable()
    {
        gameObject.transform.localPosition = scriptable.thisPose.position;
        gameObject.transform.localRotation = scriptable.thisPose.rotation;
    }

    private void SaveJson()
    {
        if (scriptable == null)
        {
            scriptable = new LocalTransformScriptable(saveName, gameObject.transform);
        }

        scriptable.UpdateValues(gameObject.transform);
        scriptable.SaveJson();
    }

    void OnApplicationQuit() {
        scriptable.UpdateValues(gameObject.transform);
        scriptable.SaveJson();
    }
}
