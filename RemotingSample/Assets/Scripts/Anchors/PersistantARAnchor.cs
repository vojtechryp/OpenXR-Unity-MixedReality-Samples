using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Microsoft.MixedReality.OpenXR.Sample
{
    [RequireComponent(typeof(ARAnchor))]
    public class PersistantARAnchor : MonoBehaviour
    {
        private ARAnchor attachedAnchor;
        public PersistentAnchorData data;
        public string PersistantAnchorName = "";

        private void OnEnable()
        {
            attachedAnchor = GetComponent<ARAnchor>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}