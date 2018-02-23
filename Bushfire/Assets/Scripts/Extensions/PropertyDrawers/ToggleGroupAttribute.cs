using UnityEngine;
using System.Collections;

public class ToggleGroupAttribute : PropertyAttribute {
	public GUIContent group_name;
	public ToggleGroupAttribute (string group_name) {
		this.group_name = new GUIContent(group_name);
	}
}
