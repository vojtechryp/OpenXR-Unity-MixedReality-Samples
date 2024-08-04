using System.Collections;
using System.Collections.Generic;
using Transform3DBestFit;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Microsoft.MixedReality.OpenXR.Sample;
using AnchorData = VRLab.AnchorStore.PersistantAnchorData;
using Unity.XR.CoreUtils;
using VRLab.AnchorStore;

namespace VRLab.QTMTracking
{
[RequireComponent(typeof(FindTransform))]
public class AnchorBasedTransform : MonoBehaviour
{
    [Header("Controls")]
    //[InspectorButton("SaveAnchorNames", ButtonWidth = 200)]
    //public bool saveAnchorNamesToScriptable = false;
    [InspectorButton("LoadAndApply", ButtonWidth = 200)]
    public bool loadAndApply = false;
    [InspectorButton("LoadFromScriptable", ButtonWidth = 200)]
    public bool loadFromNamesButton = false;
    [InspectorButton("CreateQTMPrefab", ButtonWidth = 200)]
    public bool createQTMPrefab = false;
    [InspectorButton("CreateDummyPrefab", ButtonWidth = 200)]
    public bool createDummyPrefab = false;

    public bool hasValidData = false;
    public bool hasValidDummyData = false;

    [Header("Dummy Object")]
    public AnchorQTMTransform qtmObject;
    public AnchorDummyTransform dummyObject;

    [Header("Data")]
    public AnchorTransformScriptable scriptableData = null;

    [Header("Persistent Anchor Manager")]
    public AnchorPersistence persistentAnchorManager;

    [Header("Lists")]
    public PersistableAnchorVisuals[] persistableAnchorVisuals;
    public List<string> anchorStoredNames;
    public AnchorData[] anchorDataList;

    internal FindTransform findTransform = null;
    internal Vector3[] vectorOfAnchorPositions;
    internal Vector3[] vector3sBackConvert;
    public double[,] pointsAsArray;
    internal int numberOfPoints { get => anchorStoredNames.Count;}

    private void OnEnable()
    {
        if (persistentAnchorManager == null)
        {
            persistentAnchorManager = FindObjectOfType<AnchorPersistence>();
        }
    }

    void LoadAndApply()
    {
        LoadFromScriptable();
        if (!hasValidData) return;
        CreateQTMPrefab();
        if (!qtmObject.hasValidData) return;
        if (dummyObject != null)
        {
            CreateDummyPrefab();
        }
        if (findTransform == null)
        {
            findTransform = GetComponent<FindTransform>();
        }
        findTransform.CalculateTransform(this, qtmObject, dummyObject);
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
            anchorStoredNames = scriptableData.anchorStoredNames;
            hasValidData = UpdateFromAnchorNames();
        }
    }

    public void SaveAnchorNames()
    {
        anchorStoredNames.Clear();
        foreach (var anchorVisual in  persistableAnchorVisuals)
        {
            anchorStoredNames.Add(anchorVisual.Name);
            scriptableData.anchorStoredNames = anchorStoredNames;
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
            anchorDataList[i] = persistentAnchorManager.LoadPersistentAnchorByData(new AnchorData(anchorStoredNames[i]));
            if (!anchorDataList[i].IsAnchorLoaded)
            {
                Debug.LogWarning($"Anchor with name {anchorStoredNames[i]} not properly loaded");
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
}
