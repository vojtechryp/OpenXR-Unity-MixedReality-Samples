using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmformQTM : MonoBehaviour
{
    public bool useInverse = true;

    public Transform qtmTransform;
    public Transform displayTransform;

    public FindTransform findTransform;

    public Matrix4x4 transformMatrix;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transformMatrix = findTransform.transformMatrix4x4;
        Matrix4x4 newMatrix = Matrix4x4.zero;
        if (useInverse)
        {
            newMatrix = transformMatrix.inverse * qtmTransform.localToWorldMatrix;
        }
        else
        {
            newMatrix = transformMatrix * qtmTransform.localToWorldMatrix;
        }
        displayTransform.localPosition = newMatrix.GetPosition();
        displayTransform.localRotation = newMatrix.rotation;
    }
}
