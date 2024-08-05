using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Vojta.Experiment
{
    [CreateAssetMenu(fileName = "CoilTargetsScriptable", menuName = "CoilTargets/CoilTargetsScriptable", order = 0)]
    public class CoilTargetsScriptable : ScriptableObject
    {
        [InspectorButton("CopyTransformsToPredefinedPoints", ButtonWidth = 300)]
        public bool copyTransformsToPoints = false;
        [InspectorButton("ResetToHardCodedValues", ButtonWidth = 300)]
        public bool resetToHardcodedValues = false;

        [SerializeField]
        public Transform BrainOriginTransform;

        [SerializeField]
        public List<Transform> ListOfTransforms;

        [SerializeField]
        public List<string> predefinedPointTags = new List<string>
        {
            "Frontal",
            "Occipital",
            "Parietal",
            "Left Temporal",
            "Right Temporal",
            "Additional 1",
            "Additional 2",
            "Additional 3",
            "Additional 4",
            "Additional 5"
        };

        [SerializeField]
        public List<PredefinedPointStruct> predefinedPoints = new();

        [SerializeField]
        public List<PredefinedPointStruct> predefinedPointsHardCoded = new List<PredefinedPointStruct>
        {
            new PredefinedPointStruct(new Vector3(-0.0064f, 0.0574f, -0.0423f), "LDLPFC"),
            new PredefinedPointStruct(new Vector3(0.0035f, 0.0366f, -0.20069f), "RDLPFC"),
            new PredefinedPointStruct(new Vector3(0.05314f, 0.07571f, -0.12568f), "OccipitalRight"),
            new PredefinedPointStruct(new Vector3(0.06784f, 0.0573f, -0.1156f), "OccipitalLeft"),
            new PredefinedPointStruct(new Vector3(-0.0438f, 0.0573f, -0.1155f), "TemporalRight "),
            new PredefinedPointStruct(new Vector3(0.0300f, 0.0650f, -0.1500f), "TemporalLeft"),
            new PredefinedPointStruct(new Vector3(-0.0300f, 0.0650f, -0.1500f), "ParietalRight"),
            new PredefinedPointStruct(new Vector3(0.0500f, 0.0700f, -0.1200f), "ParietalLeft"),
            new PredefinedPointStruct(new Vector3(-0.0500f, 0.0700f, -0.1200f), "PMRight"),
            new PredefinedPointStruct(new Vector3(0.0000f, 0.0600f, -0.1800f), "PMLeft")
        };

        [SerializeField]
        public List<List<PredefinedPointStruct>> predefinedPointsBackup = new List<List<PredefinedPointStruct>>();

        public void ResetToHardCodedValues()
        {
            predefinedPoints = new(predefinedPointsHardCoded);
        }

        public void CopyTransformsToPredefinedPoints()
        {
            Assert.IsTrue(ListOfTransforms.Count == 10);


            foreach (var transform in ListOfTransforms)
            {
                if (transform == null)
                {
                    return;
                }
            }

            predefinedPointsBackup.Add(new(predefinedPoints));

            predefinedPoints = new List<PredefinedPointStruct>();

            for (int i = 0; i < 10; i++)
            {
                var transform = ListOfTransforms[i];
                var tag = predefinedPointTags[i];
                var localForwardVector = BrainOriginTransform.transform.InverseTransformDirection(transform.forward); 
                predefinedPoints.Add(new PredefinedPointStruct(transform.localPosition, transform.localEulerAngles, tag));
            }
        }

        
    }
}