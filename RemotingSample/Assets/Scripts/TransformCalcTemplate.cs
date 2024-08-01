using System.Collections.Generic;
using Transform3DBestFit;
using UnityEngine;

// using Vector3 = UnityEngine.Vector3;

public class TransformCalcTemplate : MonoBehaviour
{
    public List<Transform> transforms = new List<Transform>(4);
    public Vector3[] vector3s;
    public int numberOfPoints = 0;
    public Vector3[] vector3sBackConvert;

    public double[,] pointsAsArray;

    private void OnValidate()
    {
        UpdateArray();
    }

    public double[,] UpdateArray()
    {
        numberOfPoints = transforms.Count;
        vector3s = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            vector3s[i] = transforms[i].position;
        }

        pointsAsArray = Transform3D.ConvertVector3sToArray(vector3s);
        vector3sBackConvert = Transform3D.ConvertArrayToVector3(pointsAsArray);
        return pointsAsArray;
    }
}
