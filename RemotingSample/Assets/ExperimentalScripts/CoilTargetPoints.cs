using System.Collections.Generic;
using UnityEngine;

public class CoilTargetPoints : MonoBehaviour
{
    public struct PredefinedPointStruct
    {
        public Vector3 point;
        public string tag;

        public PredefinedPointStruct(Vector3 point, string tag)
        {
            this.point = point;
            this.tag = tag;
        }
    }

    public Transform BrainTargetTransform;
    public Color sphereColor = Color.red;  // Color for the visualization spheres
    public float sphereScale = 0.0015f;  // Scale for the visualization spheres

    public static List<PredefinedPointStruct> predefinedPoints;

    void Awake()
    {
        predefinedPoints = new List<PredefinedPointStruct>
        {
            new PredefinedPointStruct(new Vector3(-0.0064f, 0.0574f, -0.0423f), "Frontal"),
            new PredefinedPointStruct(new Vector3(0.0035f, 0.0366f, -0.20069f), "Occipital"),
            new PredefinedPointStruct(new Vector3(0.05314f, 0.07571f, -0.12568f), "Parietal"),
            new PredefinedPointStruct(new Vector3(0.06784f, 0.0573f, -0.1156f), "Left Temporal"),
            new PredefinedPointStruct(new Vector3(-0.0438f, 0.0573f, -0.1155f), "Right Temporal"),
            new PredefinedPointStruct(new Vector3(0.0300f, 0.0650f, -0.1500f), "Additional 1"),
            new PredefinedPointStruct(new Vector3(-0.0300f, 0.0650f, -0.1500f), "Additional 2"),
            new PredefinedPointStruct(new Vector3(0.0500f, 0.0700f, -0.1200f), "Additional 3"),
            new PredefinedPointStruct(new Vector3(-0.0500f, 0.0700f, -0.1200f), "Additional 4"),
            new PredefinedPointStruct(new Vector3(0.0000f, 0.0600f, -0.1800f), "Additional 5")
        };
    }

    public static List<PredefinedPointStruct> GetRandomizedPoints()
    {
        List<PredefinedPointStruct> randomizedPoints = new List<PredefinedPointStruct>(predefinedPoints);
        for (int i = 0; i < randomizedPoints.Count; i++)
        {
            PredefinedPointStruct temp = randomizedPoints[i];
            int randomIndex = Random.Range(i, randomizedPoints.Count);
            randomizedPoints[i] = randomizedPoints[randomIndex];
            randomizedPoints[randomIndex] = temp;
        }
        return randomizedPoints;
    }

    public GameObject CreateVisualizationSphere(Vector3 localPosition)
    {
        Vector3 worldPosition = BrainTargetTransform.TransformPoint(localPosition); // Ensure it is relative to BrainPosition
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = worldPosition;
        sphere.transform.localScale = new Vector3(sphereScale, sphereScale, sphereScale); // Adjust scale as needed

        // Ensure the sphere is visible by setting its material color
        Renderer renderer = sphere.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = sphereColor;

        Debug.Log($"Visualization sphere created at: {worldPosition}");
        return sphere;
    }

    public static string GetBrainPositionTag(Vector3 point)
    {
        foreach (var predefinedPoint in predefinedPoints)
        {
            if (predefinedPoint.point == point)
            {
                return predefinedPoint.tag;
            }
        }
        return "Unknown";
    }
}
