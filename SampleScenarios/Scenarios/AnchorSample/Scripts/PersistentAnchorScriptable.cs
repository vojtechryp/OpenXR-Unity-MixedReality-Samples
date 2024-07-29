using Microsoft.MixedReality.OpenXR.ARFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Microsoft.MixedReality.OpenXR.Sample
{
    [CreateAssetMenu(menuName = "My Assets/AnchorScriptable")]
    public class PersistentAnchorScriptable : ScriptableObject
    {

        [SerializeField]
        public TrackableId brainTrackableID;
        [SerializeField]
        public string brainTrackableIDString;
        [SerializeField]
        public string brainTrackableNameString;
        [SerializeReference]
        public TextAsset jsonAsset;
        [SerializeField]
        public List<TrackableId> ARAnchors = new List<TrackableId>();
        [SerializeField]
        public List<string> ARAnchorStrings = new List<string>();
        
        public void UpdateStrings()
        {
            if (brainTrackableID != null)
            {
                brainTrackableIDString = brainTrackableID.ToString();
            }

            ARAnchorStrings = IdToString(ARAnchors);
        }
        public string SaveToJson()
        {

            string json = JsonUtility.ToJson(this);
            AssetDatabase.CreateAsset(new TextAsset(json), "Assets/AnchorData.asset");
            
            return "Assets/AnchorData.asset";
        }

        public List<string> IdToString(List<TrackableId> trackableIDs)
        {
            List<string> idStrings = new();
            foreach (var id in trackableIDs)
            {
                idStrings.Add(id.ToString());
            }
            return idStrings;
        }
        public void clearAnchors()
        {
            brainTrackableID = TrackableId.invalidId;
            ARAnchors.Clear();
        }

        public void overwriteFromJson(TextAsset jsonAsset)
        {
            this.jsonAsset = jsonAsset;
            JsonUtility.FromJsonOverwrite(jsonAsset.text, this);
        }
    }
}
