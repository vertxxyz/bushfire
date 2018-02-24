using BezierCurves;
using UnityEngine;

public class CurveInformation : MonoBehaviour
{
	public BezierCurve3D curve;
	public float angle;
	public float curveLength;
	[ReadOnly]
	public Transform t;

	public float intendedSpeedStart = 80;
	public float intendedSpeedEnd = 80;

	private void OnEnable()
	{
		t = transform;
	}

	[EditorButton]
	public void EstimateCurveLength()
	{
		curveLength = curve.GetApproximateLength();
	}
}
