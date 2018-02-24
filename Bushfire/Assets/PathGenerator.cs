using System.Collections.Generic;
using BezierCurves;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
	public List<BezierCurve3D> pathSegments = new List<BezierCurve3D>();

	public Queue<BezierCurve3D> curves = new Queue<BezierCurve3D>();
	
	public void AppendPath()
	{
		
	}
}
