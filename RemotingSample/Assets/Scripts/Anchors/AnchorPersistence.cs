// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.OpenXR.ARFoundation;
using Microsoft.MixedReality.OpenXR.Sample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using VRLab.QTMTracking;


//#if USE_ARFOUNDATION_5_OR_NEWER
using ARSessionOrigin = Unity.XR.CoreUtils.XROrigin;
//#else
//using ARSessionOrigin = UnityEngine.XR.ARFoundation.ARSessionOrigin;
//#endif

namespace VRLab.AnchorStore
{

    /// <summary> 
    /// This sample detects air taps, creating new unpersisted anchors at the locations. Air tapping 
    /// again near these anchors toggles their persistence, backed by the <c>XRAnchorStore</c>.
    /// </summary>
    [RequireComponent(typeof(ARAnchorManager))]
    [RequireComponent(typeof(ARSessionOrigin))]
    public class AnchorPersistence : MonoBehaviour
    {
        public bool autoLoad = false;
        private bool[] m_wasTapping = { true, true };
        public bool m_airTapToCreateEnabled = false;
        private bool m_airTapToCreateEnabledChangedThisUpdate = false;

        private ARSessionOrigin m_arSessionOrigin; // Used for ARSessionOrigin.trackablesParent
        protected ARAnchorManager m_arAnchorManager;
        private List<ARAnchor> m_anchors = new List<ARAnchor>();

        //protected XRAnchorStore m_anchorStore = null;
        protected XRAnchorStore m_anchorStore { get => PersistantAnchorStore.AnchorStore; }

        //private Dictionary<TrackableId, string> m_incomingPersistedAnchors = new Dictionary<TrackableId, string>();
        public bool hasValidStore { get => m_anchorStore != null; }

        private static async Task<bool> Pause(int delay)
        {
            await Task.Delay(delay);//does not crash here
            return true;
        }

        protected async void OnEnable()
        {
            if (!TryGetComponent(out m_arAnchorManager) || !m_arAnchorManager.enabled || m_arAnchorManager.subsystem == null)
            {
                Debug.Log($"ARAnchorManager not enabled or available; sample anchor functionality will not be enabled.");
                return;
            }

            m_arAnchorManager.anchorsChanged += AnchorsChanged;

            await PersistantAnchorStore.LoadStore();

            await Pause(1000);

            // Request all persisted anchors be loaded once the anchor store is loaded.
            if (autoLoad && m_anchorStore != null)
            {
                foreach (string name in m_anchorStore.PersistedAnchorNames)
                {
                    // When a persisted anchor is requested from the anchor store, LoadAnchor returns the TrackableId which
                    // the anchor will use once it is loaded. To later recognize and recall the names of these anchors after
                    // they have loaded, this dictionary stores the TrackableIds.
                    TrackableId trackableId = m_anchorStore.LoadAnchor(name);
                    PersistantAnchorStore.incomingPersistentAnchors.Add(trackableId, new PersistantAnchorData(name));
                }
            }
        }

        public PersistantAnchorData LoadPersistentAnchorByData(PersistantAnchorData anchorData)
        {
            UnityTransformMarkers overrideUnityMarker = anchorData.hasAssociatedQTMMarker ? anchorData.AssociatedQTMMarker : UnityTransformMarkers.Unassigned;
            if (PersistantAnchorStore.nameToDataDict.TryGetValue(anchorData.StoredName, out anchorData))
            {
                anchorData.AssociatedQTMMarker = overrideUnityMarker;
                Debug.Log($"Found existing anchor named {anchorData.StoredName}");
            } 
            else
            {
                if (hasValidStore)
                {
                    if (m_anchorStore.PersistedAnchorNames.Contains(anchorData.StoredName))
                    {
                        anchorData.AssociatedQTMMarker = overrideUnityMarker;
                        anchorData.trackableId = m_anchorStore.LoadAnchor(anchorData.StoredName);
                        PersistantAnchorStore.incomingPersistentAnchors.Add(anchorData.trackableId, anchorData);
                    }
                    else
                    {
                        Debug.LogWarning($"Persistent anchor named {anchorData.StoredName} not found in store");
                    }
                }
            }

            return anchorData;
        }

        protected void OnDisable()
        {
            if (m_arAnchorManager != null)
            {
                m_arAnchorManager.anchorsChanged -= AnchorsChanged;
                //m_anchorStore = null;
                //m_incomingPersistedAnchors.Clear();
                PersistantAnchorStore.incomingPersistentAnchors.Clear();
            }
        }

        private void AnchorsChanged(ARAnchorsChangedEventArgs eventArgs)
        {
            foreach (var added in eventArgs.added)
            {
                Debug.Log($"Anchor added from ARAnchorsChangedEvent: {added.trackableId}, OpenXR Handle: {added.GetOpenXRHandle()}");
                ProcessAddedAnchor(added);
            }

            foreach (ARAnchor updated in eventArgs.updated)
            {
                if (updated.TryGetComponent(out PersistableAnchorVisuals sampleAnchorVisuals))
                {
                    sampleAnchorVisuals.TrackingState = updated.trackingState;
                }
            }

            foreach (var removed in eventArgs.removed)
            {
                Debug.Log($"Anchor removed: {removed.trackableId}");
                m_anchors.Remove(removed);
            }
        }



        public virtual void ProcessAddedAnchor(ARAnchor anchor)
        {
            // If this anchor being added was requested from the anchor store, it is recognized here
            //if (m_incomingPersistedAnchors.TryGetValue(anchor.trackableId, out string name))
            //{
            //    AddPersistantAnchor(anchor, name);
            //    m_incomingPersistedAnchors.Remove(anchor.trackableId);
            //}

            if (PersistantAnchorStore.incomingPersistentAnchors.TryGetValue(anchor.trackableId, out PersistantAnchorData anchorData))
            {
                AddPersistantAnchor(anchor, anchorData.StoredName);
                anchorData.anchor = anchor;
                anchorData.trackableId = anchor.trackableId;

                PersistantAnchorStore.idToDataDict.Add(anchorData.trackableId, anchorData);
                PersistantAnchorStore.nameToDataDict.Add(anchorData.StoredName, anchorData);

                PersistantAnchorStore.incomingPersistentAnchors.Remove(anchor.trackableId);
            }

            void AddPersistantAnchor(ARAnchor anchor, string name)
            {
                if (anchor.TryGetComponent(out PersistableAnchorVisuals sampleAnchorVisuals))
                {
                    sampleAnchorVisuals.Name = name;
                    sampleAnchorVisuals.Persisted = true;
                    sampleAnchorVisuals.TrackingState = anchor.trackingState;
                }
            }

            m_anchors.Add(anchor);
        }

        private bool IsTapping(InputDevice device)
        {
            bool isTapping;

            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out isTapping))
            {
                return isTapping;
            }
            else if (device.TryGetFeatureValue(CommonUsages.primaryButton, out isTapping))
            {
                return isTapping;
            }
            return false;
        }

        private void LateUpdate()
        {
            // Air taps for anchor creation are handled in LateUpdate() to avoid race conditions with air taps to enable/disable anchor creation.
            for (int i = 0; i < 2; i++)
            {
                InputDevice device = InputDevices.GetDeviceAtXRNode((i == 0) ? XRNode.RightHand : XRNode.LeftHand);

                bool isTapping = IsTapping(device);
                if (isTapping && !m_wasTapping[i])
                {
                    //OnAirTapped(device);
                }
                m_wasTapping[i] = isTapping;
            }

            m_airTapToCreateEnabledChangedThisUpdate = false;
        }

        public void OnAirTapped(InputDevice device)
        {
            if (!m_arAnchorManager.enabled || m_arAnchorManager.subsystem == null)
            {
                return;
            }

            Vector3 position;
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                return;

            // First, check if there is a nearby anchor to persist/forget.
            if (m_anchors.Count > 0)
            {
                var (distance, closestAnchor) = m_anchors.Aggregate(
                    new Tuple<float, ARAnchor>(Mathf.Infinity, null),
                    (minPair, anchor) =>
                    {
                        float dist = (position - anchor.transform.position).magnitude;
                        return dist < minPair.Item1 ? new Tuple<float, ARAnchor>(dist, anchor) : minPair;
                    });

                if (distance < 0.1f)
                {
                    ToggleAnchorPersistence(closestAnchor);
                    return;
                }
            }

            // If there's no anchor nearby, create a new one.
            // If an air tap to enable/disable anchor creation just occurred, the tap is ignored here.
            if (m_airTapToCreateEnabled && !m_airTapToCreateEnabledChangedThisUpdate)
            {
                Vector3 headPosition;
                if (!InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.devicePosition, out headPosition))
                    headPosition = Vector3.zero;

                AddAnchor(new Pose(position, Quaternion.LookRotation(position - headPosition, Vector3.up)));
            }
        }

        public void AddAnchor(Pose pose)
        {
#pragma warning disable 0618 // warning CS0618: 'ARAnchorManager.AddAnchor(Pose)' is obsolete
            ARAnchor newAnchor = m_arAnchorManager.AddAnchor(pose);
#pragma warning restore 0618
            if (newAnchor == null)
            {
                Debug.Log($"Anchor creation failed");
            }
            else
            {
                Debug.Log($"Anchor created: {newAnchor.trackableId}");
            }
        }

        public ARAnchor AddAnchorReturn(Pose pose)
        {
#pragma warning disable 0618 // warning CS0618: 'ARAnchorManager.AddAnchor(Pose)' is obsolete
            ARAnchor newAnchor = m_arAnchorManager.AddAnchor(pose);
#pragma warning restore 0618
            if (newAnchor == null)
            {
                Debug.Log($"Anchor creation failed");
            }
            else
            {
                Debug.Log($"Anchor created: {newAnchor.trackableId}");
            }
            return newAnchor;
        }

        public TrackableId AddPersistentAnchor(Pose pose)
        {
#pragma warning disable 0618 // warning CS0618: 'ARAnchorManager.AddAnchor(Pose)' is obsolete
            ARAnchor newAnchor = m_arAnchorManager.AddAnchor(pose);
#pragma warning restore 0618
            if (newAnchor != null)
            {
                Debug.Log($"Anchor created: {newAnchor.trackableId}");
                ToggleAnchorPersistence(newAnchor);
                return newAnchor.trackableId;
            }
            else
            {
                Debug.Log($"Anchor creation failed");
                return TrackableId.invalidId;
            }

        }

        public void ToggleAnchorPersistence(ARAnchor anchor)
        {
            if (m_anchorStore == null)
            {
                Debug.Log($"Anchor Store was not available.");
                return;
            }

            PersistableAnchorVisuals sampleAnchorVisuals = anchor.GetComponent<PersistableAnchorVisuals>();
            if (!sampleAnchorVisuals.Persisted)
            {
                // For the purposes of this sample, randomly generate a name for the saved anchor.
                string newName = "";
                if (sampleAnchorVisuals.Name == "")
                {
                    newName = $"anchor/{Guid.NewGuid().ToString().Substring(0, 4)}";
                } else
                {
                    newName = sampleAnchorVisuals.Name;
                }

                bool succeeded = m_anchorStore.TryPersistAnchor(anchor.trackableId, newName);
                if (!succeeded)
                {
                    Debug.Log($"Anchor could not be persisted: {anchor.trackableId}");
                    return;
                }

                ChangeAnchorVisuals(anchor, newName, true);
            }
            else
            {
                m_anchorStore.UnpersistAnchor(sampleAnchorVisuals.Name);
                ChangeAnchorVisuals(anchor, "", false);
            }
        }

        public List<string> UpdatePositionsOfAnchors(List<string> anchorNames, bool newPersist)
        {
            PersistantAnchorData foundAnchor = null;
            List<string> newAnchorNames = new List<string>();

            if (!newPersist)
            {
                foreach (string name in anchorNames)
                {
                    if (PersistantAnchorStore.nameToDataDict.TryGetValue(name, out foundAnchor))
                    {
                        ToggleAnchorPersistenceCustom(foundAnchor, false);
                        newAnchorNames.Add(name);
                    }
                }
            }
            else
            {
                foreach (string name in anchorNames)
                {
                    if (PersistantAnchorStore.nameToDataDict.TryGetValue(name, out foundAnchor))
                    {
                        newAnchorNames.Add(ToggleAnchorPersistenceCustom(foundAnchor, true));
                        PersistantAnchorStore.nameToDataDict.Remove(name);
                        PersistantAnchorStore.nameToDataDict.Add(name, foundAnchor);
                    }
                }
            }

            return newAnchorNames;
        }

        public string ToggleAnchorPersistenceCustom(PersistantAnchorData anchorData, bool newPersist)
        {
            if (m_anchorStore == null)
            {
                Debug.Log($"Anchor Store was not available.");
                return anchorData.visuals.Name;
            }

            PersistableAnchorVisuals sampleAnchorVisuals = anchorData.visuals;
            if (newPersist)
            {
                // For the purposes of this sample, randomly generate a name for the saved anchor.
                string newName = $"anchor/{Guid.NewGuid().ToString().Substring(0, 4)}";

                bool succeeded = m_anchorStore.TryPersistAnchor(anchorData.trackableId, newName);
                if (!succeeded)
                {
                    Debug.Log($"Anchor could not be persisted: {anchorData.trackableId}");
                    return anchorData.visuals.Name;
                }

                ChangeAnchorVisuals(anchorData.anchor, newName, true);
                return newName;
            }
            else
            {
                m_anchorStore.UnpersistAnchor(sampleAnchorVisuals.Name);
                ChangeAnchorVisuals(anchorData.anchor, sampleAnchorVisuals.Name, false);
                return anchorData.visuals.Name;
            }
        }

        public void AnchorStoreClear()
        {
            m_anchorStore.Clear();
            // Change visual for every anchor in the scene
            foreach (ARAnchor anchor in m_anchors)
            {
                ChangeAnchorVisuals(anchor, "", false);
            }
            PersistantAnchorStore.idToDataDict.Clear();
            PersistantAnchorStore.nameToDataDict.Clear();
        }

        public void ClearSceneAnchors()
        {
            // Remove every anchor in the scene. This does not affect their persistence
            foreach (ARAnchor anchor in m_anchors)
            {
                m_arAnchorManager.subsystem.TryRemoveAnchor(anchor.trackableId);
            }
            m_anchors.Clear();
        }

        private void ChangeAnchorVisuals(ARAnchor anchor, string newName, bool isPersisted)
        {
            PersistableAnchorVisuals sampleAnchorVisuals = anchor.GetComponent<PersistableAnchorVisuals>();
            Debug.Log(isPersisted ? $"Anchor {anchor.trackableId} with name {newName} persisted" : $"Anchor {anchor.trackableId} with name {sampleAnchorVisuals.Name} unpersisted");
            sampleAnchorVisuals.Name = newName;
            sampleAnchorVisuals.Persisted = isPersisted;
        }
        public void ToggleAirTapToCreateEnabled()
        {
            m_airTapToCreateEnabled = !m_airTapToCreateEnabled;
            m_airTapToCreateEnabledChangedThisUpdate = true;
        }
    }
}
