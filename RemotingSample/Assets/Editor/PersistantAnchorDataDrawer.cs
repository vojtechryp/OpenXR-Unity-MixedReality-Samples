using UnityEngine;
using UnityEditor;
using VRLab.AnchorStore;


#if USE_PROPERTYDRAWERS
[CustomPropertyDrawer(typeof(PersistantAnchorData))]
public class PersistantAnchorDataDrawer : PropertyDrawer
{
    public override void OnGUI(
      Rect position,
      SerializedProperty prop,
      GUIContent label
      )
    {
        base.OnGUI(position, prop, label);
    }
}
#endif