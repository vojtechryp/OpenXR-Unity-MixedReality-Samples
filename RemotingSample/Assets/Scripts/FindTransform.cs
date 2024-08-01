using System.Collections;
using System.Collections.Generic;
using Transform3DBestFit;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FindTransform : MonoBehaviour
{
    public bool doCalc = true;
    public TransformCalcTemplate templateSource;
    public TransformCalcTemplate templateActual;
    public TransformCalcTemplate templateToMove;
    public Matrix4x4 transformMatrix4x4;


    private void OnValidate()
    {
        if (!templateSource || !templateActual || !templateToMove || !doCalc) return;

        Transform3D t3d = new Transform3D(templateSource.UpdateArray(), templateActual.UpdateArray());

        t3d.CalcTransform(t3d.actualsMatrix, t3d.nominalsMatrix);

        transformMatrix4x4 = ConvertToMatrix4x4(t3d.TransformMatrix);
        UpdateSlaveTransform();
        doCalc = false;
    }

    private Matrix4x4 ConvertToMatrix4x4(double[,] source)
    {
        Matrix4x4 newMatrix = Matrix4x4.identity;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                newMatrix[i, j] = (float)source[i, j];
            }
        }
        return newMatrix;
    }

    public void UpdateSlaveTransform()
    {
        Matrix4x4 newMatrix = transformMatrix4x4.inverse * templateActual.transform.localToWorldMatrix;
        templateToMove.transform.position = newMatrix.GetPosition(); 
        templateToMove.transform.rotation = newMatrix.rotation; 
    }
}
