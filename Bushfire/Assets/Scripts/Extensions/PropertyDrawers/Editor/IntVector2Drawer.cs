using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(IntVector2Attribute))]
public class IntVector2GroupDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		//IntVector2Attribute tG = attribute as IntVector2Attribute;
		Vector2 before = property.vector2Value;
		EditorGUI.PropertyField (position, property);
		Vector2 delta = property.vector2Value - before;
		delta.x = Mathf.Ceil (Mathf.Abs (delta.x)) * Mathf.Sign (delta.x);
		delta.y = Mathf.Ceil (Mathf.Abs (delta.y)) * Mathf.Sign (delta.y);
		property.vector2Value = new Vector2 (Mathf.RoundToInt (before.x+delta.x), Mathf.RoundToInt (before.y+delta.y));
	}
}
