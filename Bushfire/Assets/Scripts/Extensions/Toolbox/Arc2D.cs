using UnityEngine;
using System.Collections;

public class Arc2D {
	public Vector2 centre;
	public float angleP1;
	public float aDelta;
	public float radius;

	public Vector2 p1;
	public Vector2 p2;

	/// <summary>
	/// Initializes a new instance of the <see cref="Arc2D"/> class.
	/// </summary>
	/// <param name="p1">A point on the arc</param>
	/// <param name="p2">Another point on the arc</param>
	/// <param name="c">The centre of the circle containing the arc</param>
	public Arc2D (Vector2 p1, Vector2 p2, Vector2 c){
		this.p1 = p1;
		this.p2 = p2;
		Vector2 chord = (p2 - p1);
		Vector2 chordNormal = Vector2FromAngle(GetAngle (Vector2.right, chord) + 90);
		Vector2 chordCentre = (p1 + p2) / 2f;
		c = Vector3.Project ((c - chordCentre), chordNormal); //Making sure c is on the chord normal
		c += chordCentre;
		centre = c;

		Vector2 radius1 = p1 - centre;
		Vector2 radius2 = p2 - centre;
		aDelta = GetAngle (radius1, radius2);
		angleP1 = GetAngle (Vector2.right, radius1);
		radius = radius1.magnitude;
	}

	public Vector2 GetPosition (float t){
		return centre + Vector2FromAngle (angleP1 + aDelta * t) * radius;
	}

	#region Helper Functions
	public Vector2 Vector2FromAngle(float a){
		a = Mathf.Repeat (a, 360);
		a *= Mathf.Deg2Rad;
		return new Vector2 (Mathf.Cos (a), Mathf.Sin (a));
	}

	public float GetAngle(Vector2 v1, Vector2 v2) {
		float sign = Mathf.Sign (v1.x * v2.y - v1.y * v2.x);
		return Vector2.Angle (v1, v2) * sign;
	}
	#endregion
}
