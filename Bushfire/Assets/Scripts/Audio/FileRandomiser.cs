using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileRandomiser : FileProvider, FolderLoader.IFolderLoad
{

	public T GetRandomObject<T>() where T : Object
	{
		if (objects == null || objects.Length == 0)
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

	public override T GetObject<T>()
	{
		return GetRandomObject<T>();
	}
}
