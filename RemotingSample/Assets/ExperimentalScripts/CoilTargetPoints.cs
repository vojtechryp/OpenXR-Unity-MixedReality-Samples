using System.Collections.Generic;
using UnityEngine;

public class CoilTargetPoints : MonoBehaviour
{
    public Color sphereColor = Color.red;  // Color for the visualization spheres
    public float sphereScale = 0.0015f;  // Scale for the visualization spheres
    private List<Vector3> predefinedPoints;
    private List<string> pointTags;

    void Awake()
    {
        // Define specific points relative to the BrainPosition GameObject
        predefinedPoints = new List<Vector3>
        {
            new Vector3(-0.0064f, 0.0574f, -0.0423f),    // Frontal
            new Vector3(0.0035f, 0.0366f, -0.20069f),    // Occipital
            new Vector3(0.05314f, 0.07571f, -0.12568f),  // Parietal
            new Vector3(0.06784f, 0.0573f, -0.1156f),    // Left Temporal
            new Vector3(-0.0438f, 0.0573f, -0.1155f),    // Right Temporal
        };

        pointTags = new List<string>
        {
            "Frontal",
            "Occipital",
            "Parietal",
            "Left Temporal",
            "Right Temporal"
        };
    }

    public List<Vector3> GetRandomizedPoints()
    {
        List<Vector3> randomizedPoints = new List<Vector3>(predefinedPoints);
        for (int i = 0; i < randomizedPoints.Count; i++)
        {
            Vector3 temp = randomizedPoints[i];
            int randomIndex = Random.Range(i, randomizedPoints.Count);
            randomizedPoints[i] = randomizedPoints[randomIndex];
            randomizedPoints[randomIndex] = temp;
        }
        return randomizedPoints;
    }

    public GameObject CreateVisualizationSphere(Vector3 localPosition)
    {
        Vector3 worldPosition = transform.TransformPoint(localPosition); // Ensure it is relative to BrainPosition
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

    public string GetBrainPositionTag(Vector3 point)
    {
        int index = predefinedPoints.IndexOf(point);
        return index >= 0 ? pointTags[index] : "Unknown";
    }
}
