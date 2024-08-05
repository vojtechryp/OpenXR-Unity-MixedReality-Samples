using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vojta.Experiment
{
    public class CoilTargetPoints : MonoBehaviour
    {
        public CoilTargetsScriptable CoilTargetsScriptableLocal;

        public static CoilTargetsScriptable CoilTargetsScriptable;

        private void OnValidate()
        {
            CoilTargetsScriptable = CoilTargetsScriptableLocal;
            if (CoilTargetsScriptable == null) {
                Debug.LogError("CoilTargetsScriptable Not Set");
            }
        }

        public void Awake()
        {
            CoilTargetsScriptable = CoilTargetsScriptableLocal;
        }

        private static List<PredefinedPointStruct> predefinedPoints { get => CoilTargetsScriptable.predefinedPoints; }

        public static List<PredefinedPointStruct> GetRandomizedPoints()
        {
            List<PredefinedPointStruct> randomizedPoints = new();
            List<int> randomIndexes = GetRandomIntSequence(predefinedPoints.Count);

            for (int i = 0; i < predefinedPoints.Count; i++)
            {
                randomizedPoints.Add(predefinedPoints[randomIndexes[i]]);
            }
            return randomizedPoints;
        }

        public string GetBrainPositionTag(Vector3 point)
        {
            foreach (var predefinedPoint in predefinedPoints)
            {
                if (predefinedPoint.TargetPosition == point)
                {
                    return predefinedPoint.Tag;
                }
            }
            return "Unknown";
        }

        public static List<int> GetRandomIntSequence(int count)
        {
            List<int> numbers = new List<int>();

            for (int i = 0; i < count; i++)
            {
                numbers.Add(i);
            }

            List<int> randomSequence = new List<int>();

            //Random rnd = new Random();
            while (numbers.Count > 0)
            {
                int position = UnityEngine.Random.Range(0, numbers.Count - 1);

                randomSequence.Add(numbers[position]);
                numbers.RemoveAt(position);
            }

            return randomSequence;
        }
    }
    [Serializable]
    public struct PredefinedPointStruct
    {
        public Vector3 TargetPosition;
        public Vector3 TargetRotation;
        public string Tag;

        public PredefinedPointStruct(Vector3 targetPoint, string tag)
        {
            TargetPosition = targetPoint;
            TargetRotation = Vector3.forward;
            this.Tag = tag;

        }

        public PredefinedPointStruct(Vector3 targetPoint, Vector3 targetRotation, string tag)
        {
            TargetPosition = targetPoint;
            TargetRotation = targetRotation;
            Tag = tag;

        }
    }
}