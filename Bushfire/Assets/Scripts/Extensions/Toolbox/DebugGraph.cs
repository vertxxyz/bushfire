using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DebugGraph : MonoBehaviour {
	struct GraphPoint {
		public float v;
		public float t;
		public Color c;
	}

	const float maxTime = 10;
	const float refreshMinMaxTime = 10;
	const int size = 200;
	const int offset = 10;
	static float lastDrawnBorder = -1;

	static GraphPoint pMinValueRelative;
	static GraphPoint pMaxValueRelative;

	static List<Grapher> graphs;

	public static Camera camera_;

	public new static Camera camera {
		get {
			if (camera_ == null || !camera_.enabled) {
				Camera[] cams = GameObject.FindObjectsOfType<Camera> ();
				float renderDepth = float.NegativeInfinity;
				int renderIndex = 0;
				for (int i = 0; i < cams.Length; i++) {
					if (cams [i].enabled && cams [i].depth > renderDepth) {
						renderDepth = cams [i].depth;
						renderIndex = i;
					}
				}
				camera_ = cams [renderIndex];
			}
			return camera_;
		}
	}

	public static void Graph (float value, Color c, int id, bool relative = true) {
		if (lastDrawnBorder != Time.time) {
			float nearClip = camera.nearClipPlane * 1.1f;
			Vector3 bottomLeft = camera.ScreenToWorldPoint (new Vector3 (offset, offset, nearClip));
			Vector3 bottomRight = camera.ScreenToWorldPoint (new Vector3 (offset, offset + size, nearClip));
			Vector3 topRight = camera.ScreenToWorldPoint (new Vector3 (offset + size, offset + size, nearClip));
			Vector3 topLeft = camera.ScreenToWorldPoint (new Vector3 (offset + size, offset, nearClip));
			Debug.DrawLine (bottomLeft, bottomRight);
			Debug.DrawLine (bottomLeft, topLeft);
			Debug.DrawLine (topRight, topLeft);
			Debug.DrawLine (topRight, bottomRight);
			lastDrawnBorder = Time.time;
		}

		int index = -1;
		if (graphs == null)
			graphs = new List<Grapher> ();
		else {
			for (int i = 0; i < graphs.Count; i++) {
				if (graphs [i].id == id)
					index = i;
			}
		}

		if (index == -1) {
			Grapher g = new Grapher ();
			g.id = id;
			index = 0;
			graphs.Add (g);
		}

		graphs [index].Add (value, c);
	}

	public static void AddMarker (Color c, int id) {
		int index = -1;
		if (graphs == null)
			graphs = new List<Grapher> ();
		else {
			for (int i = 0; i < graphs.Count; i++) {
				if (graphs [i].id == id)
					index = i;
			}
		}

		if (index == -1) {
			Grapher g = new Grapher ();
			g.id = id;
			index = 0;
			graphs.Add (g);
		}

		graphs [index].AddVerticalMarker (c);
	}

	class Grapher {
		public GraphPoint pMinValue;
		public GraphPoint pMaxValue;
		public Queue<GraphPoint> queue;
		public Queue<GraphPoint> verticalMarkers;
		public float lastDrawn = -1;
		public int id;

		public bool relative = true;

		public Grapher () {
			queue = new Queue<GraphPoint> ();
			verticalMarkers = new Queue<GraphPoint> ();
		}

		public void Add (float v, Color c) {
			GraphPoint p;
			p.v = v;
			p.t = Time.time;
			p.c = c;
			queue.Enqueue (p);
			if (lastDrawn == -1 || lastDrawn != p.t)
				Draw ();
		}

		public void AddVerticalMarker (Color c) {
			GraphPoint p;
			p.t = Time.time;
			p.c = c;
			p.v = 0;
			verticalMarkers.Enqueue (p);
			if (lastDrawn == -1 || lastDrawn != p.t)
				Draw ();
		}

		public void Draw () {
			float endTime = 0;
			float maxValue = Mathf.NegativeInfinity;
			float minValue = Mathf.Infinity;
			foreach (GraphPoint p in queue) {
				endTime = p.t;
				if (p.v > maxValue)
					maxValue = p.v;
				if (p.v < minValue)
					minValue = p.v;
			}

			if (pMinValue.c == Color.white) {
				pMinValue.c = Color.red;
				pMaxValue.v = maxValue;
				pMinValue.v = minValue;
			}

			if (minValue > pMinValue.v && endTime - pMinValue.t > refreshMinMaxTime) {
				pMinValue.v = minValue;
				pMinValue.t = endTime;
			} else if (minValue < pMinValue.v) {
				pMinValue.v = minValue;
				pMinValue.t = endTime;
			}

			if (maxValue < pMaxValue.v && endTime - pMaxValue.t > refreshMinMaxTime) {
				pMaxValue.v = maxValue;
				pMaxValue.t = endTime;
			} else if (maxValue > pMaxValue.v) {
				pMaxValue.v = maxValue;
				pMaxValue.t = endTime;
			}

//			float min = pMinValue.v;
//			float max = pMaxValue.v;
//			if (relative) {
//				if (pMinValueRelative.v > pMinValue.v) {
//					pMinValueRelative.v = pMinValue.v;
//					pMinValueRelative.t = pMinValue.t;
//				}
//				if (pMaxValueRelative.v < pMaxValue.v) {
//					pMaxValueRelative.v = pMaxValue.v;
//					pMaxValueRelative.t = pMaxValue.t;
//				}
//				min = pMinValueRelative.v;
//				max = pMaxValueRelative.v;
//			}

			GraphPoint lastP = new GraphPoint ();
			lastP.t = -1;
			float startTime = queue.Peek ().t;
			float diff = endTime - startTime;
			foreach (GraphPoint p in queue) {
				if (lastP.t != -1) {
					float nearPlane = camera.nearClipPlane * 1.05f;
					Vector3 fromP = new Vector3 (-((lastP.t - startTime) - diff), lastP.v, nearPlane);
					fromP.y = offset + size * Mathf.InverseLerp (pMinValue.v, pMaxValue.v, fromP.y);
					fromP.x = offset + size * (fromP.x / (float)maxTime);
					Vector3 toP = new Vector3 (-((p.t - startTime) - diff), p.v, nearPlane);
					toP.y = offset + size * Mathf.InverseLerp (pMinValue.v, pMaxValue.v, toP.y);
					toP.x = offset + size * (toP.x / (float)maxTime);
					fromP = camera.ScreenToWorldPoint (fromP);
					toP = camera.ScreenToWorldPoint (toP);
					Debug.DrawLine (fromP, toP, p.c);
				}
				lastP = p;
			}
			foreach (GraphPoint p in verticalMarkers) {
				float nearPlane = camera.nearClipPlane * 1.05f;
				Vector3 toBP = new Vector3 (-((p.t - startTime) - diff), offset, nearPlane);
				Vector3 toTP = new Vector3 (-((p.t - startTime) - diff), offset + size, nearPlane);
				toBP.x = offset + size * (toBP.x / (float)maxTime);
				toTP.x = offset + size * (toTP.x / (float)maxTime);
				toBP = camera.ScreenToWorldPoint (toBP);
				toTP = camera.ScreenToWorldPoint (toTP);
				Debug.DrawLine (toBP, toTP, p.c);
			}
			while ((endTime - queue.Peek ().t) > maxTime) {
				queue.Dequeue ();
			}
			if (verticalMarkers.Count > 0) {
				while ((endTime - verticalMarkers.Peek ().t) > maxTime) {
					verticalMarkers.Dequeue ();
				}
			}
		}
	}
}
