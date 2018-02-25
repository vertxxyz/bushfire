using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
	public List<CurveInformation> pathSegments = new List<CurveInformation>();

	public List<CurveInformation> curves = new List<CurveInformation>();
	private CurveInformation lastAssignedCurve;

	public int pathLength = 50;

	private Transform t;

	public Transform carTransform;
	public SteeringWheel steeringWheel;
	
	private void Start()
	{
		t = transform;
		lastAssignedCurve = Instantiate(pathSegments[Random.Range(0, pathSegments.Count)]);
		currentCurve = 0;
		lastAssignedCurve.transform.SetParent(t);
		curves.Add(lastAssignedCurve);
		for (int i = 0; i < pathLength; i++)
		{
			AppendPath();
		}
	}

	private void AppendPath()
	{
		float currentAngle = 0;
		float currentAngle20 = 0;
		
		//Only check the 10 last roads to make sure we aren't making a boo boo
		for (int i = curves.Count-1; i>=0; i--)
		{
			currentAngle += curves[i].angle;
			if (curves.Count - i == 20)
				currentAngle20 = currentAngle;
		}

		CurveInformation curveInfoToSpawn = null;
		while (curveInfoToSpawn == null)
		{
			curveInfoToSpawn = pathSegments[Random.Range(0, pathSegments.Count)];
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Mathf.Abs(curveInfoToSpawn.angle)>15 && curveInfoToSpawn.angle == lastAssignedCurve.angle)
			{
				curveInfoToSpawn = null;
			}
			else
			{
				float newAngle = currentAngle + curveInfoToSpawn.angle;
				if (newAngle < -90 || newAngle > 90)
				{
					curveInfoToSpawn = null;
				}
				else
				{
					float newAngle20 = currentAngle20 + curveInfoToSpawn.angle;
					if (newAngle20 < -60 || newAngle20 > 60)
					{
						curveInfoToSpawn = null;
					}
				}
			}
		}
		
		CurveInformation newCurve = Instantiate(curveInfoToSpawn);
		curves.Add(newCurve);
		newCurve.t.position = lastAssignedCurve.curve.KeyPoints[lastAssignedCurve.curve.KeyPointsCount - 1].Position;
		newCurve.t.rotation = lastAssignedCurve.t.rotation * Quaternion.LookRotation(lastAssignedCurve.curve.KeyPoints[lastAssignedCurve.curve.KeyPointsCount - 1].LeftHandleLocalPosition.normalized, Vector3.up);
		lastAssignedCurve = newCurve;
		lastAssignedCurve.t.SetParent(t);
	}

	public float speedMultiplier = 1;
	public float acceleration = 1;
	public float decelleration = 1;
	[ReadOnly]
	public float currentSpeed;

	private int currentCurve;
	private float currentCurveValue = 0;

	public float apprehensionDistance = 0.5f;
	
	private void Update()
	{
		//Apprehend the speed apprehensionDistance*currentSpeed down the track
		int currentApprehensionCurve = currentCurve;
		float curveLengthRemaining = (1-currentCurveValue) * curves[currentApprehensionCurve].curveLength;
		float lastApprehensionDistanceRemaining = apprehensionDistance * currentSpeed;
		float apprehensionDistanceRemaining = lastApprehensionDistanceRemaining - curveLengthRemaining;
		float apprehensionValueInternal = currentCurveValue;
		while (apprehensionDistanceRemaining > 0)
		{
			currentApprehensionCurve++;
			lastApprehensionDistanceRemaining = apprehensionDistanceRemaining;
			apprehensionDistanceRemaining -= curves[currentApprehensionCurve].curveLength;
			apprehensionValueInternal = 0;
		}

		float currentApprehensionValue = apprehensionValueInternal + lastApprehensionDistanceRemaining / curves[currentApprehensionCurve].curveLength;
		float speedToApprehend = Mathf.Lerp(curves[currentApprehensionCurve].intendedSpeedStart,
			curves[currentApprehensionCurve].intendedSpeedEnd, currentApprehensionValue);
		Debug.DrawRay(curves[currentApprehensionCurve].curve.GetPoint(currentApprehensionValue), Vector3.up*speedToApprehend * 0.1f, Color.magenta);
		

		//Change speed
		float intendedSpeed = Mathf.Lerp(curves[currentCurve].intendedSpeedStart, curves[currentCurve].intendedSpeedEnd,
			currentCurveValue);
		intendedSpeed = Mathf.Min(speedToApprehend, intendedSpeed);
		float accelerate = (intendedSpeed>currentSpeed) ? acceleration : decelleration;
		
		currentSpeed = Mathf.MoveTowards(currentSpeed, intendedSpeed, accelerate * Time.deltaTime * Mathf.Abs(currentSpeed-intendedSpeed));
		
		//Move Down Curve / Next Curve
		float curveSpeed = (currentSpeed * speedMultiplier) / curves[currentCurve].curveLength;
		currentCurveValue += curveSpeed * Time.deltaTime;
		while (currentCurveValue > 1)
		{
			float leftOverDistance = (currentCurveValue - 1) * curves[currentCurve].curveLength;
			CyclePathIfNeeded();
			currentCurve++;
			currentCurveValue = leftOverDistance / curves[currentCurve].curveLength;
		}
		
		//Move all curves
		Vector3 newPoint = curves[currentCurve].curve.GetPoint(currentCurveValue);
		Vector3 newNormal = curves[currentCurve].curve.GetTangent(currentCurveValue);
		Vector3 fromToPos = -newPoint;
		carTransform.rotation = Quaternion.LookRotation(-newNormal, Vector3.up);
		foreach (var curve in curves)
		{
			curve.t.position += fromToPos;
		}

		steeringWheel.UpdateAngle(curves[currentCurve].angle);
	}

	private void CyclePathIfNeeded()
	{
		if (currentCurve >= pathLength/2)
		{
			Destroy(curves[0].gameObject);
			curves.RemoveAt(0);
			currentCurve--;
			AppendPath();
		}
	}

	[EditorButton]
	void Regenerate()
	{
		for (int i = curves.Count - 1; i >= 0; i--)
		{
			Destroy(curves[i].gameObject);
			curves.RemoveAt(i);
		}
		Start();
	}
}
