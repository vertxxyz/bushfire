using UnityEngine;
using System.Collections;

public class DirectoryAttribute : PropertyAttribute
{
	public readonly bool unityDirectory;
	public DirectoryAttribute (bool unityDirectory)
	{
		this.unityDirectory = unityDirectory;
	}
}
