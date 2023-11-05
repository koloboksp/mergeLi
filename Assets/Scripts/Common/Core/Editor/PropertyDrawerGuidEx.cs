using UnityEngine;
using UnityEditor;
using Atom;

namespace Assets.Scripts.Common.Shared
{ 
    [CustomPropertyDrawer(typeof(GuidEx))]
    public class PropertyDrawerGuidEx : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var oldGuid = (GuidEx)property.GetTargetObjectOfProperty();

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.TextField(position, oldGuid.ToString());

            if (Conversion.IsGuidEx(result))
            {
                var newGuid = new GuidEx(result);

                if (oldGuid != newGuid)
                {
                    property.SetTargetObjectOfProperty(newGuid);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
            else
                property.SetTargetObjectOfProperty(oldGuid);
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}