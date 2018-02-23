using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileRandomiser : MonoBehaviour, FolderLoader.IFolderLoad, IFileProvider
{

	public T GetRandomObject<T>() where T : Object
	{
		if (objects == null)
			return null;
		Object o = objects[Random.Range(0, objects.Length)];
		T t = o as T;
		if(t == null)
			Debug.LogWarning("Provided object " + o + " is not of type " + typeof(T).Name);
		return t;
	}
	
	public Object[] objects;
	public void LoadObjects(Object[] objects)
	{
		this.objects = objects;
	}

	public T GetObject<T>() where T : Object
	{
		return GetRandomObject<T>();
	}
}
