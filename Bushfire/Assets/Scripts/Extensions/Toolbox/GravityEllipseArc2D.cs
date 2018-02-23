using UnityEngine;
using System.Collections;

/// <summary>
/// An arc of an ellipse between two points that will always "hang" down.
/// </summary>
public class GravityEllipseArc2D {
	private float startAngle;
	private int dir;
	private Vector3 centre;
	private float w;
	private float h;
	public GravityEllipseArc2D (Vector2 fromPos, Vector2 toPos){
		h = Mathf.Abs (toPos.y - fromPos.y);
		w = Mathf.Abs (toPos.x - fromPos.x);
		centre = Vector3.zero;
		startAngle = - Mathf.PI / 2f;
		dir = 1;
		if (fromPos.y >= toPos.y) {
			centre = new Vector3 (toPos.x, fromPos.y, 0);
			if (toPos.x >= fromPos.x) {
				startAngle = Mathf.PI;
			} else {
				startAngle = 0;
			}
		} else {
			centre = new Vector3 (fromPos.x, toPos.y, 0);
		}
		if (toPos.x < fromPos.x) {
			dir = -1;
		}
	}

	public Vector2 GetPosition (float t){
		return centre + new Vector3 (w * Mathf.Cos (dir * t * (Mathf.PI / 2f) + startAngle), h * Mathf.Sin (dir * t * (Mathf.PI / 2f) + startAngle));
	}
}
