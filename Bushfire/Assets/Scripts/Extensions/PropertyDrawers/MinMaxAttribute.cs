using UnityEngine;
using System.Collections;

public class MinMaxAttribute : PropertyAttribute {
	public float min;
	public float max;
	public GUIContent label;
	public MinMaxAttribute (string label, float min, float max) {
		this.label = new GUIContent(label);
		this.min = min;
		this.max = max;
	}
}
