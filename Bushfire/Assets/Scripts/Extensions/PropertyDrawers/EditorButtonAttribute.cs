// Initial Concept by http://www.reddit.com/user/zaikman
// Revised by http://www.reddit.com/user/quarkism

using UnityEngine;

/// <summary>
/// Stick this on a method
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method)]
public class EditorButtonAttribute : PropertyAttribute
{
	public float r;
	public float g;
	public float b;
	public float a;
	public Color c {
		get {
			return new Color (r, g, b, a);
		}
	}

	public EditorButtonAttribute () : this(1,1,1,1) {}
	public EditorButtonAttribute (float r, float g, float b, float a){
		this.r = r;
		this.b = b;
		this.g = g;
		this.a = a;
	}
}