using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour
{
    public Camera cameraObj;
    public GameObject myGameObj;
    public float speed = 2f;

    void Update()
    {
        RotateCameraUpdate();
    }

    void RotateCameraUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            cameraObj.transform.RotateAround(myGameObj.transform.position,
                                            cameraObj.transform.up,
                                            Input.GetAxis("Mouse X") * speed);

            cameraObj.transform.RotateAround(myGameObj.transform.position,
                                            cameraObj.transform.right,
                                            -Input.GetAxis("Mouse Y") * speed);

        }

    }
}