using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(LabelAttribute))]
public class LabelAttributeDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		LabelAttribute a = attribute as LabelAttribute;
		EditorGUI.PropertyField (position, property, a.label);
	}
}
