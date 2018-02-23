using UnityEngine;
using UnityEditor;
using System.Collections;

public class HandleExtensions : MonoBehaviour {
	/// <summary>
	/// Arc2DHandle.
	/// </summary>
	/// <returns>Returns the center position used to draw an Arc.</returns>
	/// <param name="p1">Point 1.</param>
	/// <param name="p2">Point 2.</param>
	/// <param name="centre">Centre.</param>
	/// <param name="wasHit">The tracking for whether the handle was selected.</param>
	static Arc2D DrawArc2DHandle (Arc2D arc, Vector2 centre, int hash, Color lineColor, Color arcColor, Event e){
		Vector3[] positions = new Vector3[20];
		for (int i = 0; i < 20; i++) {
			positions [i] = arc.GetPosition (i / 19f);
			positions [i] = positions [i];
		}
		Handles.color = arcColor;
		Handles.DrawAAPolyLine (positions);
		Handles.color = lineColor;
		Handles.DrawLine (arc.centre, arc.p1);
		Handles.DrawLine (arc.centre, arc.p2);
		Handles.DrawDottedLine (arc.centre, centre, 5f);
		float size = HandleUtility.GetHandleSize (centre) * 0.1f;
		if (e.type == EventType.Layout)
			HandleUtility.AddControl (hash, HandleUtility.DistanceToRectangle (Handles.matrix.MultiplyPoint (centre), Quaternion.identity, size));
		Handles.RectangleHandleCap (hash, centre, Quaternion.identity, size, EventType.Repaint);
		return arc;
	}

	public static bool DoArc2DHandleInteraction(Arc2D arc, ref Vector2 centre, int hash, bool wasSelectedLast, Color lineColor, Color arcColor){
		Event e = Event.current;
		DrawArc2DHandle(arc, centre, hash, lineColor, arcColor, e);
		if (wasSelectedLast || (HandleUtility.nearestControl == hash && e.type == e.GetTypeForControl (hash) && e.type == EventType.MouseDown)) {
			Ray r = HandleUtility.GUIPointToWorldRay (e.mousePosition);
			Plane p = new Plane (Vector3.forward, 0);
			float outHit = 0;
			if (p.Raycast (r, out outHit)) {
				Vector2 pHit = r.origin + r.direction * outHit;
				centre = pHit;
				return true;
			}
		}
		return false;
	}
}
