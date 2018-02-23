using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ToggleGroupAttribute))]
public class ToggleGroupDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		ToggleGroupAttribute tG = attribute as ToggleGroupAttribute;


		float w = GUI.skin.label.CalcSize(tG.group_name).x;
		w = Mathf.Max (w, position.width / 4f);
		Rect r_toggle = new Rect (position.position, new Vector2(w+27, position.height));
		if (property.propertyType == SerializedPropertyType.Boolean)
			property.boolValue = EditorGUI.ToggleLeft (r_toggle, tG.group_name, property.boolValue);
		else
			Debug.LogError ("ToggleGroupDrawer's attribute must be a Boolean value.");
		
		property.Next (false);

		position.x = position.x + r_toggle.width;
		position.width -= r_toggle.width;
		EditorGUI.PropertyField (position, property, GUIContent.none);
	}
}
