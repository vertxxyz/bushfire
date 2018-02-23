using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(NextPrevAttribute))]
public class NextPrevDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		NextPrevAttribute nP = attribute as NextPrevAttribute;

		if (GUI.Button (new Rect (position.x, position.y, position.width / 3f, position.height), "Previous")) {
			property.intValue = Mathf.Clamp(property.intValue - 1, nP.min, nP.max);
		}
		GUI.enabled = false;
		GUI.Label(new Rect (position.x+position.width / 3f, position.y, position.width / 3f, position.height), property.intValue.ToString());
		GUI.enabled = true;
		if (GUI.Button (new Rect (position.x+position.width / 3f+position.width / 3f, position.y, position.width / 3f, position.height), "Next")) {
			property.intValue = Mathf.Clamp(property.intValue + 1, nP.min, nP.max);
		}
	}
}
