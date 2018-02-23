using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
[CustomEditor(typeof (MonoBehaviour), true)]
public class EditorButton : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		var mono = target as MonoBehaviour;

		var methods = mono.GetType()
			.GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
				BindingFlags.NonPublic)
			.Where(o => Attribute.IsDefined(o, typeof (EditorButtonAttribute)));

		foreach (var memberInfo in methods)
		{
			EditorButtonAttribute editorButton = (EditorButtonAttribute)Attribute.GetCustomAttribute (memberInfo, typeof(EditorButtonAttribute));
			GUI.color = editorButton.c;
			if (GUILayout.Button(memberInfo.Name))
			{
				var method = memberInfo as MethodInfo;
				method.Invoke(mono, null);
			}
			GUI.color = Color.white;
		}
	}
}