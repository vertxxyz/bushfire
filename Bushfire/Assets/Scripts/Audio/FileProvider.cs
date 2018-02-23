using UnityEngine;

public interface IFileProvider
{
	T GetObject<T> () where T : Object;
}
