// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//#if USE_ARFOUNDATION_5_OR_NEWER
//using ARSessionOrigin = Unity.XR.CoreUtils.XROrigin;
//#else
//using ARSessionOrigin = UnityEngine.XR.ARFoundation.ARSessionOrigin;
//#endif

namespace Microsoft.MixedReality.OpenXR.Sample
{
    [Serializable]
    public class PersistentAnchorData
    {
        public TrackableId trackableId = TrackableId.invalidId;
        public string name = "";
        public ARAnchor anchor = null;
        public bool isAnchorLoaded { get => anchor != null; }
        public bool isIdValid { get => trackableId != TrackableId.invalidId; }
        public PersistableAnchorVisuals visuals { get => anchor.GetComponent<PersistableAnchorVisuals>(); }

        public static Dictionary<string, PersistentAnchorData> nameToDataDict = new Dictionary<string, PersistentAnchorData>();
        public static Dictionary<TrackableId, PersistentAnchorData> idToDataDict = new Dictionary<TrackableId, PersistentAnchorData>();

        public static Dictionary<TrackableId, PersistentAnchorData> incomingPersistentAnchors = new Dictionary<TrackableId, PersistentAnchorData>();
        public PersistentAnchorData(TrackableId _trackableId, string _name)
        {
            trackableId = _trackableId;
            name = _name;
        }
        public PersistentAnchorData(string _name)
        {
            name = _name;
        }
    }
}
