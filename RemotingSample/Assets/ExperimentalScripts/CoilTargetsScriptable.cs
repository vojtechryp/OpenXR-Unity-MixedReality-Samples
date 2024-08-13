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
        //[InspectorButton("CopyTransformsToPredefinedPoints", ButtonWidth = 300)]
        //public bool copyTransformsToPoints = false;
        //[InspectorButton("ResetToHardCodedValues", ButtonWidth = 300)]
        //public bool resetToHardcodedValues = false;

        [SerializeField]
        public List<PredefinedPointStruct> predefinedPoints = new();

        //[SerializeField]
        //public Transform BrainOriginTransform;

        //[SerializeField]
        //public List<Transform> ListOfTransforms;

        [SerializeField]
        public List<string> predefinedPointTags = new List<string>
        {
            "LDLPFC",
            "RDLPFC",
            "OccipitalRight",
            "OccipitalLeft",
            "TemporalRight",
            "TemporalLeft",
            "ParietalRight",
            "ParietalLeft",
            "PMRight",
            "PMLeft"
        };


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

        //public void CopyTransformsToPredefinedPoints()
        //{
        //    Assert.IsTrue(ListOfTransforms.Count == 10);


        //    foreach (var transform in ListOfTransforms)
        //    {
        //        if (transform == null)
        //        {
        //            return;
        //        }
        //    }

        //    predefinedPointsBackup.Add(new(predefinedPoints));

        //    predefinedPoints = new List<PredefinedPointStruct>();

        //    for (int i = 0; i < 10; i++)
        //    {
        //        var transform = ListOfTransforms[i];
        //        var tag = predefinedPointTags[i];
        //        var localForwardVector = BrainOriginTransform.transform.InverseTransformDirection(transform.forward); 
        //        predefinedPoints.Add(new PredefinedPointStruct(transform.localPosition, transform.localEulerAngles, tag));
        //    }
    //    //}

    //    GenericPropertyJSON:{"name":"predefinedPoints","type":-1,"arraySize":10,"arrayType":"PredefinedPointStruct","children":[{"name":"Array","type":-1,"arraySize":10,"arrayType":"PredefinedPointStruct","children":[{"name":"size","type":12,"val":10},{"name":"data","type":-1,"children":[{"name":"TargetPosition","type":9,"children":[{"name":"x","type":2,"val":-0.02282},{ "name":"y","type":2,"val":0.06766},{ "name":"z","type":2,"val":-0.05068}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":320.964233},{ "name":"y","type":2,"val":313.0073},{ "name":"z","type":2,"val":75.5605}]},{ "name":"Tag","type":3,"val":"LDLPFC"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":0.02022},{ "name":"y","type":2,"val":0.06739},{ "name":"z","type":2,"val":-0.05073}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":324.683746},{ "name":"y","type":2,"val":46.8649521},{ "name":"z","type":2,"val":139.5831}]},{ "name":"Tag","type":3,"val":"RDLPFC"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":0.03186},{ "name":"y","type":2,"val":0.00747},{ "name":"z","type":2,"val":-0.20007}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":354.061859},{ "name":"y","type":2,"val":129.856262},{ "name":"z","type":2,"val":115.192307}]},{ "name":"Tag","type":3,"val":"OccipitalRight"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":-0.0334},{ "name":"y","type":2,"val":0.0078},{ "name":"z","type":2,"val":-0.2}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":334.685333},{ "name":"y","type":2,"val":195.089645},{ "name":"z","type":2,"val":95.35429}]},{ "name":"Tag","type":3,"val":"OccipitalLeft"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":0.0577},{ "name":"y","type":2,"val":0.0379},{ "name":"z","type":2,"val":-0.1044}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":354.167633},{ "name":"y","type":2,"val":88.99608},{ "name":"z","type":2,"val":117.314171}]},{ "name":"Tag","type":3,"val":"TemporalRight"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":-0.0563},{ "name":"y","type":2,"val":0.0378},{ "name":"z","type":2,"val":-0.1044}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":334.685333},{ "name":"y","type":2,"val":282.929749},{ "name":"z","type":2,"val":95.35427}]},{ "name":"Tag","type":3,"val":"TemporalLeft"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":0.0338},{ "name":"y","type":2,"val":0.0808},{ "name":"z","type":2,"val":-0.1081}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":299.76944},{ "name":"y","type":2,"val":64.27186},{ "name":"z","type":2,"val":32.098896}]},{ "name":"Tag","type":3,"val":"ParietalRight"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":-0.0296},{ "name":"y","type":2,"val":0.0786},{ "name":"z","type":2,"val":-0.1093}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":308.1559},{ "name":"y","type":2,"val":303.439026},{ "name":"z","type":2,"val":137.750015}]},{ "name":"Tag","type":3,"val":"ParietalLeft"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":0.0063},{ "name":"y","type":2,"val":0.0871},{ "name":"z","type":2,"val":-0.0713}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":295.94458},{ "name":"y","type":2,"val":9.21499},{ "name":"z","type":2,"val":79.9056244}]},{ "name":"Tag","type":3,"val":"PMRight"}]},{ "name":"data","type":-1,"children":[{ "name":"TargetPosition","type":9,"children":[{ "name":"x","type":2,"val":-0.0063},{ "name":"y","type":2,"val":0.0871},{ "name":"z","type":2,"val":-0.0713}]},{ "name":"TargetRotation","type":9,"children":[{ "name":"x","type":2,"val":295.94458},{ "name":"y","type":2,"val":9.21499},{ "name":"z","type":2,"val":79.9056244}]},{ "name":"Tag","type":3,"val":"PMLeft"}]}]}]} 
    }
}
