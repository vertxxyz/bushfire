using UnityEngine;
using System.Collections;

public class LabelAttribute : PropertyAttribute {
	public GUIContent label;
	public LabelAttribute (string label){
		this.label = new GUIContent(label);
	}
}
