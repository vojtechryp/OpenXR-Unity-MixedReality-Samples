using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ConvergeneCalibrationScript : MonoBehaviour
{
    [Header("Controls")]
    [InspectorButton("DoCalculation")]
    public bool recalculate = false;
    public bool doCalibration = false;
    public int numberOfRepeats = 4;

    [Header("Results")]
    [SerializeField]
    public List<Vector2> calibration;

    [Header("Transforms")]
    public Transform mainCamTransform;
    public Transform calibrationTransform;
    public Transform fingerTipTransform;

    [Header("Recorded Poses")]
    public List<Pose> calibrationPoses;
    public List<Pose> fingerTipPoses;


    private bool listenForKeyPress = false;
    private Pose latestRecordedPose;

    private void OnValidate()
    {
        DoCalculation();
    }

    public void DoCalculation()
    {
        if (calibrationPoses.Count == fingerTipPoses.Count)
        {
            calibration = new();

            for (int i = 0; i < calibrationPoses.Count; i++)
            {
                calibration.Add(new Vector2(calibrationPoses[i].position.z, calibrationPoses[i].position.z - fingerTipPoses[i].position.z));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (doCalibration)
        {
            doCalibration = false;
            StartCoroutine(doCalibrationCoroutine());
        }

        if (listenForKeyPress)
        {
            if (Input.GetKeyUp(KeyCode.Space)) {
                latestRecordedPose = fingerTipTransform.GetWorldPose();
                Vector3 fingerTipInCameraFrame = mainCamTransform.InverseTransformPoint(latestRecordedPose.position);
                latestRecordedPose.position = fingerTipInCameraFrame;
                listenForKeyPress = false;
            }
        }
    }

    IEnumerator doCalibrationCoroutine()
    {
        Debug.Log($"Calibration Started");
        fingerTipPoses.Clear();
        for (int repeat = 0; repeat < numberOfRepeats; repeat++)
        {
            for (int i = 0; i < calibrationPoses.Count; i++)
            {
                Debug.Log($"Calibrating point {i}");

                calibrationTransform.localPosition = calibrationPoses[i].position;
                listenForKeyPress = true;
                yield return new WaitWhile(() => listenForKeyPress);
                
                if (repeat == 0)
                {
                    fingerTipPoses.Add(latestRecordedPose);
                } else 
                {
                    Vector3 newPosition = fingerTipPoses[i].position + latestRecordedPose.position;
                    Pose newPose = fingerTipPoses[i];
                    newPose.position = newPosition/2.0f;
                    fingerTipPoses[i] = newPose;
                }
                
                
                
                yield return null;
            }
        }


        Debug.Log($"Calibration Finished");

        yield return null;
    }
}
