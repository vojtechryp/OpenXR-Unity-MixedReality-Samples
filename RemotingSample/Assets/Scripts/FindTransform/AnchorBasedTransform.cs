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
		public Vector3[] vectorOfAnchorPositions;
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

		IEnumerator LoadCoroutine()
		{
			float waitTime = 0.2f;
			LoadFromScriptable();
			if (!hasValidData) yield break;

            yield return new WaitForSeconds(waitTime);

			CreateQTMPrefab();
			if (!qtmObject.hasValidData) yield break;

            yield return new WaitForSeconds(waitTime);

            if (dummyObject != null)
			{
				CreateDummyPrefab();
			}

			yield return new WaitForSeconds(waitTime);

			if (findTransform == null)
			{
				findTransform = GetComponent<FindTransform>();
			}

			yield return new WaitForSeconds(waitTime);

			findTransform.CalculateTransform(this, qtmObject, dummyObject);
		}

		void LoadAndApply()
		{
				StartCoroutine(LoadCoroutine());
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
					//anchorDataList[i] = persistentAnchorManager.LoadPersistentAnchorByData(new AnchorData(anchorStoredNames[i]));
				string anchorName = scriptableData.correspondingPoints[i].anchorStoredName;
				UnityTransformMarkers associatedUnityMarker = scriptableData.correspondingPoints[i].UnityMarkerName;
					anchorDataList[i] = persistentAnchorManager.LoadPersistentAnchorByData(new AnchorData(anchorName, associatedUnityMarker));
				if (!anchorDataList[i].IsAnchorLoaded)
				{
					Debug.LogWarning($"Anchor with name {anchorStoredNames[i]} not properly loaded");
					break;
					}
				ARAnchor thisAnchor = anchorDataList[i].anchor;
				persistableAnchorVisuals[i] = thisAnchor.GetComponent<PersistableAnchorVisuals>();
				vectorOfAnchorPositions[i] = thisAnchor.transform.position;
			}

			UpdateArrays();

			return true;
		}

			private void Update()
			{
				for (int i = 0; i < numberOfPoints; i++)
				{
					vectorOfAnchorPositions[i] = persistableAnchorVisuals[i].transform.position;
				}
			}

			public double[,] UpdateArrays()
			{
				for (int i = 0; i < numberOfPoints; i++)
				{
						vectorOfAnchorPositions[i] = persistableAnchorVisuals[i].transform.position;
				}
				pointsAsArray = Transform3D.ConvertVector3sToArray(vectorOfAnchorPositions);
				vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
				return pointsAsArray;
			}
	}
}
