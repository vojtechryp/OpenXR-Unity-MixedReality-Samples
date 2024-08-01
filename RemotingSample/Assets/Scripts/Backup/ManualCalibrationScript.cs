using System.IO;
using UnityEngine;

public class ManualCalibrationScript : MonoBehaviour
{
    public bool doCalibration = false;

    public Transform headsetTransform;
    public Transform brainCalibTransform; // BRAIN_MANUAL_CALIB_SET_FROM_MARKER
    public Transform brainManualCalibTransform; // BRAIN_MANUAL_CALIB_SET_MANUALLY

    public float moveScaling = 0.001f;
    public float rotateScaling = 1.0f;

    private string calibrationFilePath;

    // Static positions and rotations to be added
    private Vector3 staticPosition = new Vector3(-0.465f, -0.11f, 1.21f);
    private Vector3 staticRotationEuler = new Vector3(0f, 157.87f, 0f);
    private Quaternion staticRotation;

    // Start is called before the first frame update
    void Start()
    {
        //// Set the path for the calibration file
        //calibrationFilePath = Path.Combine(Application.persistentDataPath, "ManualCalibrationData.json");

        //if (brainCalibTransform == null)
        //{
        //    Debug.LogError("Brain Calibration Transform (BRAIN_MANUAL_CALIB_SET_FROM_MARKER) is not assigned.");
        //}

        //if (brainManualCalibTransform == null)
        //{
        //    Debug.LogError("Brain Manual Calibration Transform (BRAIN_MANUAL_CALIB_SET_MANUALLY) is not assigned.");
        //}

        //// Convert static rotation from Euler to Quaternion
        //staticRotation = Quaternion.Euler(staticRotationEuler);

        //// Load the calibration data from the file
        //LoadCalibration();
    }

    // Update is called once per frame
    void Update()
    {
        if (!doCalibration) { return; }

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveTransform(brainCalibTransform, Vector3.back);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            moveTransform(brainCalibTransform, Vector3.forward);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            moveTransform(brainCalibTransform, Vector3.right);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            moveTransform(brainCalibTransform, Vector3.left);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            moveTransform(brainCalibTransform, Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            moveTransform(brainCalibTransform, Vector3.down);
        }


        // ROTATE
        if (Input.GetKeyDown(KeyCode.T))
        {
            rotateTransform(brainCalibTransform, Vector3.back);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            rotateTransform(brainCalibTransform, Vector3.forward);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            rotateTransform(brainCalibTransform, Vector3.right);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            rotateTransform(brainCalibTransform, Vector3.left);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            rotateTransform(brainCalibTransform, Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            rotateTransform(brainCalibTransform, Vector3.down);
        }

        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    Debug.Log("N key pressed - Adjusting position.");
        //    AdjustManualCalibrationPosition();
        //}

        //// Save calibration data when 'M' key is pressed
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    Debug.Log("M key pressed - Saving calibration.");
        //    SaveCalibration();
        //}
    }

    public void moveTransform(Transform transform, Vector3 moveDirection)
    {
        Vector3 moveDirectionLocal = moveDirection * moveScaling;
        transform.localPosition += moveDirectionLocal;
    }

    public void rotateTransform(Transform transform, Vector3 rotateDirection)
    {
        transform.Rotate(rotateDirection, rotateScaling);
    }

    // Method to save the calibration data to a JSON file
    private void SaveCalibration()
    {
        // Update the data object with the current transform values
        Vector3 position = brainManualCalibTransform.localPosition;
        Quaternion rotation = brainManualCalibTransform.localRotation;

        // Log the transform values being saved
        Debug.Log($"Saving Calibration - Position: {position}, Rotation: {rotation}");

        // Create a data object to hold the transform data
        TransformData data = new TransformData
        {
            position = position,
            rotation = rotation
        };

        // Serialize the data to JSON and save it to a file
        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(calibrationFilePath, jsonString);
        Debug.Log("Calibration saved to " + calibrationFilePath);
        Debug.Log("Calibration data: " + jsonString);
    }

    // Method to load the calibration data from a JSON file
    private void LoadCalibration()
    {
        if (File.Exists(calibrationFilePath))
        {
            string jsonString = File.ReadAllText(calibrationFilePath);
            TransformData data = JsonUtility.FromJson<TransformData>(jsonString);

            // Apply the loaded data to the brainManualCalibTransform
            brainManualCalibTransform.localPosition = data.position;
            brainManualCalibTransform.localRotation = data.rotation;

            // Log the transform values being loaded
            Debug.Log($"Loaded Calibration - Position: {data.position}, Rotation: {data.rotation}");

            Debug.Log("Calibration loaded from " + calibrationFilePath);
        }
        else
        {
            Debug.LogWarning("Calibration file not found at " + calibrationFilePath);
        }
    }

    // Method to localize the marker
    private void LocalizeMarker()
    {
        // Here you can add your logic to localize the marker
        // For now, this method just logs the current position and rotation of brainCalibTransform
        Debug.Log($"Marker Localized - Position: {brainCalibTransform.localPosition}, Rotation: {brainCalibTransform.localRotation}");
    }

    // Method to add static position and rotation to the marker's position
    private void AdjustManualCalibrationPosition()
    {
        // Calculate the new position by adding the static position to the current position of brainCalibTransform
        brainManualCalibTransform.localPosition = brainCalibTransform.localPosition + staticPosition;
        brainManualCalibTransform.localRotation = brainCalibTransform.localRotation * staticRotation;

        // Log the adjusted transform values
        Debug.Log($"Adjusted Calibration - Position: {brainManualCalibTransform.localPosition}, Rotation: {brainManualCalibTransform.localRotation}");
    }

    // Class to hold transform data for serialization
    // Note from Genia: This is essentially what the "Pose" class already does?
    [System.Serializable]
    public class TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
