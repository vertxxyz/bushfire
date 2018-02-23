using UnityEngine;

public abstract class FileProvider : MonoBehaviour
{
	public abstract T GetObject<T> () where T : Object;
}
