// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.OpenXR.Sample;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using VRLab.QTMTracking;

namespace VRLab.AnchorStore
{
    [Serializable]
    public class PersistantAnchorData
    {
        // KEY PERSISTANT PARAMETERS SAVED 
        [Header("Serialized Data")]
        [SerializeField]
        [ShowOnly]
        private string name = "";
        [SerializeField]
        [ShowOnly]
        private string tag = "";
        [SerializeField]
        public UnityTransformMarkers AssociatedQTMMarker = 0;

        [Header("Runtime Data")]
        public ARAnchor anchor = null;

        [Header("Display Only Data")]
        [ShowOnly] internal bool isARAnchorLoaded = false;
        [ShowOnly] internal bool isPersistant = false;
        [ShowOnly] internal bool hasAssociatedQTMMarker = false;

        // RUNTIME CALCULATED PROPERTIES        
        public string Name { get => name; }
        public string Tag { get => tag; }
        public string StoredName { get => $"{tag}/{name}"; }
        public TrackableId TrackableId
        {
            get => IsAnchorLoaded ? anchor.trackableId : trackableId;
            set => anchor = PersistantAnchorStore.GetAnchorFromARAnchorManager(trackableId = value);
        }
        public TrackableId trackableId = TrackableId.invalidId;
        public bool IsAnchorLoaded { get => anchor != null; }
        public bool HasValidTrackableId { get => TrackableId != TrackableId.invalidId; }
        public PersistableAnchorVisuals visuals { get => anchor.GetComponent<PersistableAnchorVisuals>(); }
        public PersistableAnchorControls controls { get => anchor.GetComponent<PersistableAnchorControls>(); }

        // Constructors
        public PersistantAnchorData(string _fullName) : this(_fullName.GetTagFromRawName(), _fullName.GetNameFromRawName()){}
        public PersistantAnchorData((string _tag, string _name) fullNameTuple) : this(fullNameTuple._tag, fullNameTuple._name){}
        public PersistantAnchorData(string _tag, string _name)
        {
            name = _name;
            tag = _tag;
        }
    }
}
