using System.Collections;
using System.Collections.Generic;
using Transform3DBestFit;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Microsoft.MixedReality.OpenXR.Sample;
using AnchorData = Microsoft.MixedReality.OpenXR.Sample.PersistentAnchorData;
using UnityEngine.XR.OpenXR.Input;
using Unity.XR.CoreUtils;
public class AnchorBasedTransform : MonoBehaviour
{
    [Header("Controls")]
    //[InspectorButton("SaveAnchorNames", ButtonWidth = 200)]
    //public bool saveAnchorNamesToScriptable = false;
    [InspectorButton("LoadFromNames", ButtonWidth = 200)]
    public bool loadFromNamesButton = false;
    [InspectorButton("CreateQTMPrefab", ButtonWidth = 200)]
    public bool createQTMPrefab = false;
    [InspectorButton("CreateDummyPrefab", ButtonWidth = 200)]
    public bool createDummyPrefab = false;
    //[InspectorButton("UpdateAnchorPositions", ButtonWidth = 200)]
    //public bool updateAnchorPositions = false;
    //[InspectorButton("UnpersistAnchors", ButtonWidth = 200)]
    //public bool unpersistAnchors = false;

    public bool hasValidData = false;
    public bool hasValidDummyData = false;

    [Header("Dummy Object")]
    public AnchorDummyTransform dummyObject;
    public AnchorQTMTransform qtmObject;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Persistent Anchor Manager")]
    public AnchorPersistence persistentAnchorManager;

    [Header("Lists")]
    public PersistableAnchorVisuals[] persistableAnchorVisuals;
    public List<string> anchorNames;
    public AnchorData[] anchorDataList;
    public Vector3[] vectorOfAnchorPositions;
    public Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;
    public int numberOfPoints { get => anchorNames.Count;}

    private void OnEnable()
    {
        if (persistentAnchorManager == null)
        {
            persistentAnchorManager = FindObjectOfType<AnchorPersistence>();
        }
    }

    public void UnpersistAnchors()
    {
        anchorNames = persistentAnchorManager.UpdatePositionsOfAnchors(anchorNames, true);
    }

    public void UpdateAnchorPositions()
    {
        persistentAnchorManager.AnchorStoreClear();
        persistentAnchorManager.ClearSceneAnchors();

        for (int i = 0; i < anchorDataList.Length; i++)
        {
            AnchorData anchorData = anchorDataList[i];
            anchorData.anchor = persistentAnchorManager.AddAnchorReturn(dummyObject.prefabTransforms[i].GetWorldPose());
            persistentAnchorManager.ToggleAnchorPersistence(anchorData.anchor);
            persistableAnchorVisuals[i] = anchorData.visuals;
            anchorNames[i] = anchorData.visuals.Name;
        }
    }

    // Start is called before the first frame update
    void LoadFromNames()
    {
        LoadFromScriptable();
    }

    public void CreateDummyPrefab()
    {
        dummyObject.scriptableData.vectorOfAnchorPositions = vectorOfAnchorPositions;
        hasValidDummyData = dummyObject.LoadFromScriptable();
    }
    public void CreateQTMPrefab()
    {
        hasValidDummyData = qtmObject.LoadFromScriptable();
    }
    public void LoadFromScriptable()
    {
        if (scriptableData != null)
        {
            anchorNames = scriptableData.anchorNames;
            hasValidData = UpdateFromAnchorNames();
        }
    }

    public void SaveAnchorNames()
    {
        anchorNames.Clear();
        foreach (var anchorVisual in  persistableAnchorVisuals)
        {
            anchorNames.Add(anchorVisual.Name);
            scriptableData.anchorNames = anchorNames;
        }
    }

    public void ClearNonNameLists()
    {
        persistableAnchorVisuals = new PersistableAnchorVisuals[numberOfPoints];
        anchorDataList = new AnchorData[numberOfPoints];
        vectorOfAnchorPositions = new Vector3[numberOfPoints];
        vector3sBackConvert = new Vector3[numberOfPoints];
        pointsAsArray = new double[numberOfPoints, 3];
    }

    public bool UpdateFromAnchorNames()
    {
        hasValidData = false;
        if (numberOfPoints == 0) return false;

        ClearNonNameLists();
        for (int i = 0; i < numberOfPoints; i++)
        {
            anchorDataList[i] = persistentAnchorManager.LoadPersistentAnchorByData(new AnchorData(anchorNames[i]));
            if (!anchorDataList[i].isAnchorLoaded)
            {
                Debug.LogWarning($"Anchor with name {anchorNames[i]} not properly loaded");
                return false;
            }
            ARAnchor thisAnchor = anchorDataList[i].anchor;
            persistableAnchorVisuals[i] = thisAnchor.GetComponent<PersistableAnchorVisuals>();
            vectorOfAnchorPositions[i] = thisAnchor.transform.position;
        }

        UpdateArrays();

        return true;
    }

    public double[,] UpdateArrays()
    {
        pointsAsArray = Transform3D.ConvertVector3sToArray(vectorOfAnchorPositions);
        vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
        return pointsAsArray;
    }
}
