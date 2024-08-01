using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Unity.XR.CoreUtils;

namespace Microsoft.MixedReality.OpenXR.Sample
{
public class FingerTipTrackingData : MonoBehaviour
{
    IMixedRealityHandJointService handJointService;
    public static Pose fingerTipPose;
    public Transform fingerTipDisplay;

    // Start is called before the first frame update
    void Start()
    {
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
    }

    // Update is called once per frame
    void Update()
    {
        if (handJointService != null)
        {
            Transform jointTransform = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Toolkit.Utilities.Handedness.Right);
            fingerTipPose = jointTransform.GetWorldPose();
            if (fingerTipDisplay != null)
            {
                fingerTipDisplay.position = fingerTipPose.position;
                fingerTipDisplay.rotation = fingerTipPose.rotation;
            }
        } else 
        {
            handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        }
    }    
}
}