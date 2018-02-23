using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class FolderLoader : MonoBehaviour
{
	[Directory(true)] public string directoryToLoadFrom;

	public IFolderLoad[] folderLoadees;
	private void Reset()
	{
		folderLoadees = GetComponents<IFolderLoad>();
	}

	#if UNITY_EDITOR
	[EditorButton]
	public void LoadFromDirectory()
	{
		if (!Directory.Exists(directoryToLoadFrom))
		{
			Debug.LogError("No directory exists : " + directoryToLoadFrom + " on object " + gameObject.name, gameObject);
			return;
		}

		string[] files = Directory.GetFiles(directoryToLoadFrom);
		List<Object> objects = new List<Object>();
		foreach (string file in files)
		{
			if (file.EndsWith(".meta"))
				continue;
			objects.Add(AssetDatabase.LoadAssetAtPath<Object>(file));
		}

		if (folderLoadees == null || folderLoadees.Length == 0)
		{
			folderLoadees = GetComponents<IFolderLoad>();
		}

		if (folderLoadees == null || folderLoadees.Length == 0)
		{
			Debug.LogError("There is no component on " + gameObject.name + " with the interface IFolderLoad", gameObject);
		}
		else
		{
			foreach (var folderLoadee in folderLoadees)
			{
				folderLoadee.LoadObjects(objects.ToArray());
			}
		}
	}
	#endif

	public interface IFolderLoad
	{
		void LoadObjects(Object[] objects);
	}
}