using UnityEngine;
using UnityEditor;

namespace Alchemy {
	[CustomPropertyDrawer(typeof(KeyCodeAttribute))]
	public class KeyCodeAttributeDrawer : PropertyDrawer {
		const float widthInput = 18;
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
			//KeyCodeAttribute a = attribute as KeyCodeAttribute;
			position = EditorGUI.PrefixLabel (position, new GUIContent(property.displayName));
			position.width = position.width - widthInput;
	
			GUI.color = Color.white;
			EditorGUI.PropertyField (position, property, GUIContent.none);
			position.x += position.width;
			position.width = widthInput;
	
			int id = EditorGUIUtility.GetControlID ((int)position.x, FocusType.Keyboard, position);
			if (Event.current.type == EventType.MouseDown && position.Contains (Event.current.mousePosition)) {
				EditorGUIUtility.keyboardControl = id;
				Event.current.Use ();
			}
	
			GUI.color = EditorGUIUtility.keyboardControl == id ? Color.green : Color.white;
			position.y -= 2;
			position.x += 1;
			GUI.Label (position, GUIContent.none, (GUIStyle)"IN ObjectField");
			if (EditorGUIUtility.keyboardControl == id && Event.current.type == EventType.KeyUp) {
				if (Event.current.keyCode != KeyCode.Escape)
					property.enumValueIndex = KeyCodeToEnumIndex (property, Event.current.keyCode);
				Event.current.Use ();
				EditorGUIUtility.keyboardControl = -1;
			} else if (EditorGUIUtility.keyboardControl == id && Event.current.isKey)
				Event.current.Use ();
			GUI.color = Color.white;
		}
	
		public int KeyCodeToEnumIndex (SerializedProperty keyCodeProperty, KeyCode keyCode){
			string[] keyCodeNames = keyCodeProperty.enumNames;
			for (int i = 0; i < keyCodeNames.Length; i++) {
				if (keyCodeNames [i].CompareTo (keyCode.ToString ()) == 0)
					return i;
			}
			return 0;
		}
	}
}