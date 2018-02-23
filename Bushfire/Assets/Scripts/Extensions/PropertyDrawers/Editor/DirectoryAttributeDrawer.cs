using UnityEngine;
using UnityEditor;
using System.IO;

[CustomPropertyDrawer(typeof(DirectoryAttribute))]
public class DirectoryAttributeDrawer : PropertyDrawer {
	
	/// <summary>
	/// Provide a text field that operates like a button that opens a folder dialogue
	/// A help box is shown when the resulting string isn't a folder directory or is incorrect
	/// </summary>
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label){
		DirectoryAttribute dA = attribute as DirectoryAttribute;
		Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		EditorGUI.LabelField(r, property.displayName, property.stringValue, EditorStyles.textField);
		r.y += EditorGUIUtility.singleLineHeight;
		
		if (position.height > EditorGUIUtility.singleLineHeight)
		{
			DirectoryButton(property, dA, "Assets");
			r.height *= 2;
			EditorGUI.HelpBox(r, "Directory is invalid or empty", MessageType.Error);
			DirectoryButton(property, dA, Application.dataPath);
		}
		else
		{
			DirectoryButton(property, dA, property.stringValue);
		}
	}

	private static void DirectoryButton(SerializedProperty sP, DirectoryAttribute dA, string path)
	{
		if (!GUIExtensions.ButtonOverPreviousControl()) return;
		
		string newDirectory = EditorUtility.OpenFolderPanel("Choose Directory", path, path.Equals("Assets") ? string.Empty : path);
		if (string.IsNullOrEmpty(newDirectory))
			return;
		if (dA.unityDirectory)
		{
			if (!newDirectory.StartsWith(Application.dataPath))
			{
				Debug.LogWarning("Directory must be local to project, eg. Assets...");
				return;
			}

			sP.stringValue = "Assets" + newDirectory.Substring(Application.dataPath.Length);
		}
		else
		{
			sP.stringValue = newDirectory;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if(string.IsNullOrEmpty(property.stringValue) || !Directory.Exists(property.stringValue))
			return EditorGUIUtility.singleLineHeight * 3;
		DirectoryAttribute dA = attribute as DirectoryAttribute;
		if(dA.unityDirectory && property.stringValue.StartsWith(Application.dataPath))
			return EditorGUIUtility.singleLineHeight * 3;
		return EditorGUIUtility.singleLineHeight;
	}
}
