using UnityEngine;
using System.Collections;

public static class TouchInputWrapper {

	public static bool GetTouchDown() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetMouseButtonDown(0);
		// If touch based device
		#else
		if(Input.touchCount == 0) {
			return false;
		}
		return Input.GetTouch(0).phase == TouchPhase.Began;
		#endif
	}
	public static bool GetTouchUp() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetMouseButtonUp(0);
		// If touch based device
		#else
		if(Input.touchCount == 0) {
			return false;
		}
		return Input.GetTouch(0).phase == TouchPhase.Ended;
		#endif
	}
	public static Vector2 GetTouchDelta() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		// If touch based device
		#else
		if(Input.touchCount == 0) {
			return Vector2.zero;
		}
		Touch first = Input.GetTouch(0);
		return first.deltaPosition * 0.01f;
		#endif
	}
	public static Vector2 GetTouchAbs() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.mousePosition;
		
		// If touch based device
		#else
		if(Input.touchCount == 0) {
			return Vector2.zero;
		}
		Touch first = Input.GetTouch(0);
		return first.position;
		#endif
	}
	public static bool ShouldBeSpinning() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetMouseButton(1);
		// If touch based device
		#else
		return Input.touchCount > 1;
		#endif
	}
	public static float GetTouchSpinDelta() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetAxis("Mouse X") * 3;
		
		// If touch based device
		#else
		// Get the equidistant point
		// find out how far each finger has rotated around that point
		// add them together
		if(Input.touchCount < 2) {
			return 0;
		}
		Touch touchA = Input.GetTouch(0);
		Touch touchB = Input.GetTouch(1);
		
		Vector2 equiPoint = Vector2.Lerp(touchB.position, touchA.position, 0.5f);
		Vector2 aStart = equiPoint - (touchA.position - touchA.deltaPosition);
		Vector2 aEnd = equiPoint - (touchA.position);
		Vector2 bStart = equiPoint - (touchB.position - touchB.deltaPosition);
		Vector2 bEnd = equiPoint - (touchB.position);
		float angA = Mathf.Atan2(aEnd.y, aEnd.x) - Mathf.Atan2(aStart.y, aStart.x);
		float angB = Mathf.Atan2(bEnd.y, bEnd.x) - Mathf.Atan2(bStart.y, bStart.x);
		float totAng = (angA + angB) * 0.2f * Mathf.Rad2Deg;
		// Make sure the deltas are moving in opposite directions!
		//float angMult = Vector2.Dot(touchA.deltaPosition, touchB.deltaPosition);
		//totAng = totAng * Mathf.InverseLerp(1, -1, angMult);
		return -totAng;
		#endif
	}
	public static float GetZoomDelta() {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetAxis("Mouse ScrollWheel") * 3;
		
		// If touch based device
		#else
		if(Input.touchCount < 2) {
			return 0;
		}
		Touch touchA = Input.GetTouch(0);
		Touch touchB = Input.GetTouch(1);
		
		Vector2 aStart = (touchA.position - touchA.deltaPosition);
		Vector2 aEnd = (touchA.position);
		Vector2 bStart = (touchB.position - touchB.deltaPosition);
		Vector2 bEnd = (touchB.position);
		return (Vector2.Distance(aEnd, bEnd) - Vector2.Distance(aStart, bStart)) * 0.001f;
		#endif
	}
	public static bool GetTouch(int num) {
		// If mouse-based device
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return Input.GetMouseButton(num);

		// If touch based device
		#else
		if(Input.touchCount == 0) {
			return false;
		}
		Touch first = Input.GetTouch(num);
		return first.phase == TouchPhase.Moved || first.phase == TouchPhase.Stationary;
		#endif
	}
}
