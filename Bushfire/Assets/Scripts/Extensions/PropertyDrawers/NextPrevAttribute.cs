using UnityEngine;
using System.Collections;

public class NextPrevAttribute : PropertyAttribute {
	public int min;
	public int max;
	public NextPrevAttribute (int min, int max) {
		this.min = min;
		this.max = max;
	}
}
