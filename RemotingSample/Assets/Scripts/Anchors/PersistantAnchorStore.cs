// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Sample;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//#if USE_ARFOUNDATION_5_OR_NEWER
using ARSessionOrigin = Unity.XR.CoreUtils.XROrigin;
//#else
//using ARSessionOrigin = UnityEngine.XR.ARFoundation.ARSessionOrigin;
//#endif

namespace Microsoft.MixedReality.OpenXR.Sample
{
    [RequireComponent(typeof(AnchorPersistence))]
    [RequireComponent(typeof(ARSessionOrigin))]
    public class PersistantAnchorStore : MonoBehaviour
    {
        [Header("Controls")]
        public bool anchorStoreLoaded = false;

        [Header("Data")]
        public List<string> AvailableAnchorsInStore;

        public GlobalPersistenceScriptable globalData;

        // Private Auto Loaded
        private static XRAnchorStore m_anchorStore;
        private ARSessionOrigin m_arSessionOrigin;
        private ARAnchorManager m_arAnchorManager;
        private static PersistantAnchorStore instance;

        public static XRAnchorStore AnchorStore { get => m_anchorStore; set => m_anchorStore = value; }
        public static bool AnchorStoreLoaded { get => m_anchorStore != null ? true : false; }

        private void OnEnable()
        {
            if (instance != null)
            {
                if (instance != this) Debug.LogWarning("Persistant anchor store: Another instance has already been loaded. This has been replaced");
            }
            instance = this;
        }

        public void Update()
        {
            anchorStoreLoaded = AnchorStore != null ? true : false;
        }

        public static async Task<bool> LoadStore()
        {
            if (m_anchorStore != null) { return true; }

            return await loadAnchorStore();
        }

        private static async Task<bool> loadAnchorStore()
        {
            if (m_anchorStore != null) { return true; }
            // Set up references in this script to ARFoundation components on this GameObject.
            instance.m_arSessionOrigin = instance.GetComponent<ARSessionOrigin>();
            if (!instance.TryGetComponent(out instance.m_arAnchorManager) || !instance.m_arAnchorManager.enabled || instance.m_arAnchorManager.subsystem == null)
            {
                Debug.Log($"ARAnchorManager not enabled or available; sample anchor functionality will not be enabled.");
                return false;
            }

            m_anchorStore = await XRAnchorStore.LoadAnchorStoreAsync(instance.m_arAnchorManager.subsystem);

            if (m_anchorStore == null)
            {
                Debug.Log("XRAnchorStore not available, sample anchor persistence functionality will not be enabled.");
                return false;
            }

            return true;
        }








        protected void OnDestroy()
        {
            m_anchorStore = null;
        }
    }
}
