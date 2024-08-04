// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.OpenXR;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


//#if USE_ARFOUNDATION_5_OR_NEWER
using ARSessionOrigin = Unity.XR.CoreUtils.XROrigin;
//#else
//using ARSessionOrigin = UnityEngine.XR.ARFoundation.ARSessionOrigin;
//#endif

namespace VRLab.AnchorStore
{
    [RequireComponent(typeof(AnchorPersistence))]
    [RequireComponent(typeof(ARSessionOrigin))]
    public class PersistantAnchorStore : MonoBehaviour
    {
        [Header("Controls")]
        [ShowOnly]
        private bool anchorStoreLoadedDisplay = false;

        [Header("Data")]
        public List<string> AnchorTagsForThisProject;
        public List<PersistantAnchorData> AvailableAnchors;

        [ShowOnly]
        public List<string> AvailableAnchorNames;
        public static List<string> AvailableAnchorsInStore { get {return Instance.AvailableAnchorNames; } }

        public GlobalPersistanceScriptable globalSavedData;

        // Private Auto Loaded
        private ARSessionOrigin m_arSessionOrigin;
        private ARAnchorManager m_arAnchorManager;

        // Private static access
        private static XRAnchorStore m_anchorStore;
        private static PersistantAnchorStore Instance;

        public static XRAnchorStore AnchorStore { get => m_anchorStore; set => m_anchorStore = value; }
        public static bool AnchorStoreLoaded { get => m_anchorStore != null ? true : false; }

        // LOOKUP DICTIONARIES
        public static Dictionary<string, PersistantAnchorData> nameToDataDict = new Dictionary<string, PersistantAnchorData>();
        public static Dictionary<TrackableId, PersistantAnchorData> idToDataDict = new Dictionary<TrackableId, PersistantAnchorData>();
        public static Dictionary<TrackableId, PersistantAnchorData> incomingPersistentAnchors = new Dictionary<TrackableId, PersistantAnchorData>();

        private async void OnEnable()
        {
            StartCoroutine(CheckIfAnchorStoreLoadedCoroutine());
            if (Instance != null)
            {
                if (Instance != this) Debug.LogWarning("Persistant anchor store: Another instance has already been loaded. This has been replaced");
            }
            Instance = this;
            await loadAnchorStore();

            AnchorPersistence anchorPersistence;
            if (TryGetComponent(out anchorPersistence))
            {
                if (anchorPersistence.enabled == false) anchorPersistence.enabled = true;
            }
        }

        public static ARAnchor GetAnchorFromARAnchorManager(TrackableId trackableId)
        {
            return Instance.m_arAnchorManager.GetAnchor(trackableId);
        }

        // STATIC UPDATERS
        private static void FetchAnchorDataFromScriptable()
        {

        }
        
        public static void SaveGlobalDataSnapshot()
        {
            Instance.globalSavedData.SaveYourself();
        }

        public static async Task<bool> LoadStore()
        {
            return await loadAnchorStore();
        }

        private static async Task<bool> loadAnchorStore()
        {
            if (m_anchorStore != null) { return true; }

            // Set up references in this script to ARFoundation components on this GameObject.
            Instance.m_arSessionOrigin = Instance.GetComponent<ARSessionOrigin>();
            if (!Instance.TryGetComponent(out Instance.m_arAnchorManager) || !Instance.m_arAnchorManager.enabled || Instance.m_arAnchorManager.subsystem == null)
            {
                Debug.LogWarning($"ARAnchorManager not enabled or available; sample anchor functionality will not be enabled.");
                return false;
            }

            m_anchorStore = await XRAnchorStore.LoadAnchorStoreAsync(Instance.m_arAnchorManager.subsystem);

            if (m_anchorStore == null)
            {
                Debug.LogWarning("XRAnchorStore not available, sample anchor persistence functionality will not be enabled.");
                return false;
            }

            // Make a local copy of the available anchor names for display only
            if (Instance != null)
            {
                Instance.globalSavedData.UpdateFromAnchorStore(m_anchorStore.PersistedAnchorNames, Instance.AnchorTagsForThisProject);
                
                Instance.AvailableAnchorNames =  Instance.globalSavedData.RawAnchorNamesInStoreOnLastUpdate;
            }

            Debug.Log("XRAnchorStore loaded succesfully");

            return true;
        }


        // MISC
        private void OnDisable() {
            StopAllCoroutines();
        }

        protected void OnDestroy()
        {
            m_anchorStore = null;
        }



        IEnumerator CheckIfAnchorStoreLoadedCoroutine()
        {
            while (isActiveAndEnabled) 
            {
                anchorStoreLoadedDisplay = AnchorStoreLoaded;
                yield return new WaitForSeconds(0.2f);
            }
            yield return null;
        }
    }
}
