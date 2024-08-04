using UnityEngine;
using UnityEditor;
using System;
using VRLab.QTMTracking;
using static VRLab.QTMTracking.QTMStaticData;

#if DONT_USE_CUSTOM
[CustomPropertyDrawer(typeof(SelectableStringListWrapper))]
public class QTMMarkerDictEntryStructDrawer : PropertyDrawer
{
    string[] options = QTMMarkerNameList.ToArray();

    public override void OnGUI(
      Rect position,
      SerializedProperty prop,
      GUIContent label
      )
    {
        prop = prop.FindPropertyRelative("MarkerDisplayName");
        string markerDisplayName = prop.stringValue;
        int index = Array.IndexOf(options, markerDisplayName);
        if (index != -1)
        {
            index = EditorGUI.Popup(position, index, options);
            // index = EditorGUI.Popup(position, label.text, index, options);
            prop.stringValue = options[index];
        }
    }
}
#endif