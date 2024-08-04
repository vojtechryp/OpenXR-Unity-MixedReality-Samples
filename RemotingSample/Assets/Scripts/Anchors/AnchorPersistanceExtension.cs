using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

namespace VRLab.AnchorStore
{
    public class AnchorPersistanceExtension : AnchorPersistence
    {
        [InspectorButton("CalculateTransform", ButtonWidth = 200)]
        public bool clearMarkers = false;

        public bool AddMarkerWithM = true;

        public Transform fingerTipTransform;
        public GameObject fingerTipDisplay;

        public List<ARAnchor> NewlyAddedAnchors;

        public int numberOfNewAnchors { get => NewlyAddedAnchors.Count;}

        private void Update()
        {

        }

        public void ClearAllMarkersButton()
        {
            clearMarkers = false;
            ClearSceneAnchors();
            AnchorStoreClear();
        }

        [System.Obsolete]
        private void LateUpdate() {
            fingerTipDisplay.SetActive(AddMarkerWithM);

            if (AddMarkerWithM )
            {
                if (Input.GetKeyUp(KeyCode.M)) {
                    AddAnchor(fingerTipTransform.GetWorldPose());
                }
                if (Input.GetKeyUp(KeyCode.N))
                {
                    if (numberOfNewAnchors > 0)
                    {
                        ARAnchor latestAdded = NewlyAddedAnchors[numberOfNewAnchors - 1];
                        NewlyAddedAnchors.Remove(latestAdded);
                        m_arAnchorManager.RemoveAnchor(latestAdded);
                    }
                }
                if (Input.GetKeyUp(KeyCode.B))
                {
                    ARAnchor latestAdded = NewlyAddedAnchors[numberOfNewAnchors - 1];
                    NewlyAddedAnchors.Remove(latestAdded);
                    foreach (ARAnchor anchor in NewlyAddedAnchors)
                    {
                        m_arAnchorManager.RemoveAnchor(anchor);
                    }
                    NewlyAddedAnchors.Clear();
                    ToggleAnchorPersistence(latestAdded);
                }
            }
        }

        public override void ProcessAddedAnchor(ARAnchor anchor)
        {
            base.ProcessAddedAnchor(anchor);
            NewlyAddedAnchors.Add(anchor);
        }
    }
}
