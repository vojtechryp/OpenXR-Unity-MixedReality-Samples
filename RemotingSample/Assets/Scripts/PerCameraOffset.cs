using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerCameraOffset : MonoBehaviour
{
    public Transform original;
    public Transform offset;
    public Transform mainCam;
    public float offsetAmountValue = 0.0f;
    public float offsetAmountValuePrevious = 0.0f; 
    public static float offsetAmountInCameraFrameStatic = -0.03f;
    public Vector3 originalInCameraFrame;
    public Vector3 originalWithOffset;

    private void OnValidate() {
        if (mainCam == null)
        {
            mainCam = Camera.main.transform;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        offsetAmountValue = offsetAmountInCameraFrameStatic;
        offsetAmountValuePrevious = offsetAmountValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (offsetAmountValue != offsetAmountValuePrevious)
        {
            offsetAmountInCameraFrameStatic = offsetAmountValue;
        }


        originalInCameraFrame = mainCam.InverseTransformPoint(original.position);
        Vector3 originalWithOffsetLocal = originalInCameraFrame + originalInCameraFrame.normalized * offsetAmountInCameraFrameStatic;
        originalWithOffset = mainCam.TransformPoint(originalWithOffsetLocal);
        offset.position = originalWithOffset;


    }

    private void LateUpdate() {
        offsetAmountValuePrevious = offsetAmountValue;
        offsetAmountValue = offsetAmountInCameraFrameStatic;
    }
}
