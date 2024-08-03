using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.OpenXR.Sample
{
    public class PersistentAnchorManager : MonoBehaviour
    {
        [Header("Controls")]
        public bool allowEdit = true;
        [Header("Data")]
        public TextAsset anchorJson;
        public PersistentAnchorScriptable anchorScriptable;
        public ARAnchor brainAnchor = null;
        public List<ARAnchor> ARAnchors;
        [Header("Refs")]
        public AnchorPersistenceSample anchorPersistenceSample;
        public ARAnchorManager anchorManager;

        [Header("Transforms")]
        public Transform brainTransform;

        private void OnValidate()
        {
            if (anchorJson)
            {
                anchorScriptable.overwriteFromJson(anchorJson);
            }
        }

        public void clearAllAnchors()
        {
            if (!allowEdit) return;
            anchorPersistenceSample.ClearSceneAnchors();
            anchorPersistenceSample.AnchorStoreClear();
        }

        public void clearSceneAnchors()
        {
            if (!allowEdit) return;
            anchorPersistenceSample.ClearSceneAnchors();
        }

        public void setBrainAnchor()
        {
            if (!allowEdit) return;

            if (brainAnchor == null)
            {
                //TrackableId newBrainAnchorID = anchorPersistenceSample.AddPersistentAnchor(brainTransform.GetWorldPose());
                //brainAnchor = anchorManager.GetAnchor(newBrainAnchorID);
            }

            if (brainAnchor != null)
            {
                anchorScriptable.brainTrackableID = brainAnchor.trackableId;
                anchorScriptable.brainTrackableIDString = brainAnchor.trackableId.ToString();

                PersistableAnchorVisuals sampleAnchorVisuals = brainAnchor.GetComponent<PersistableAnchorVisuals>();
                string anchorName = sampleAnchorVisuals.Name;
                anchorScriptable.brainTrackableNameString = anchorName;

            }

        }

        public void setOtherAnchors()
        {
            if (!allowEdit) return;

            anchorScriptable.ARAnchors.Clear();
            foreach (var anchor in ARAnchors)
            {
                anchorScriptable.ARAnchors.Add(anchor.trackableId);
                anchorScriptable.ARAnchorStrings.Add(anchor.trackableId.ToString());
            }
        }

        public void SaveAnchors()
        { 
            if (!allowEdit) return;
            
            anchorScriptable.SaveToJson();
        }

        public void clearBrainAnchor()
        {
            //if (anchorScriptable.brainTrackableIDString != null)
            //{
            //    TrackableId brainID = new TrackableId(anchorScriptable.brainTrackableIDString);
            //    ARAnchor brainAnchor = anchorManager.GetAnchor(brainID);
            //    anchorPersistenceSample.m_anchorStore.UnpersistAnchor(brainAnchor.name);
            //}
        }

        public void LoadAnchors()
        {
            anchorScriptable.overwriteFromJson(anchorJson);
            if (anchorScriptable.brainTrackableNameString != null)
            {
                string anchorName = anchorScriptable.brainTrackableNameString;
                //var Anchors = GetComponents<ARAnchor>();
                var anchors = FindObjectsOfType<ARAnchor>();
                foreach (var anchor in anchors)
                {
                    var brainVisuals = anchor.gameObject.GetComponent<PersistableAnchorVisuals>();
                    if (brainVisuals != null)
                    {
                        if (anchorName == brainVisuals.Name)
                        {
                            Debug.Log("Brain Anchor Found");
                            brainAnchor = anchor;
                            Transform brainAnchorTransform = brainAnchor.gameObject.transform;
                            brainTransform.position = brainAnchorTransform.position;
                            brainTransform.rotation = brainAnchorTransform.rotation;
                            return;
                        }
                    }
                }
                Debug.Log("Brain Anchor not found by name");
            }
            Debug.Log("Brain Anchor name not in data");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadAnchors();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                setBrainAnchor();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                setOtherAnchors();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveAnchors();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                clearAllAnchors();
            }
        }
    }
}
