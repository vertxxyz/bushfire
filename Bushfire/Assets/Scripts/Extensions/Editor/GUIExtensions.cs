using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Net;
using System.Reflection;

public static class GUIExtensions
{
	#region Exponential Slider

	public static void ExponentialSlider(SerializedProperty property, float ymin, float ymax, params GUILayoutOption[] options)
	{
		property.floatValue = ExponentialSlider(new GUIContent(property.displayName), property.floatValue, ymin, ymax, options);
	}

	/// <summary>
	/// Exponential slider.
	/// </summary>
	/// <returns>The exponential value</returns>
	/// <param name="val">Value.</param>
	/// <param name="ymin">Ymin at x = 0.</param>
	/// <param name="ymax">Ymax at x = 1.</param>
	public static float ExponentialSlider(float val, float ymin, float ymax, params GUILayoutOption[] options)
	{
		val = Mathf.Clamp(val, ymin, ymax);
		ymin -= 1; //Because ur face that's why
		//val is "y"
		float x = Mathf.Log(val - ymin) / Mathf.Log(ymax - ymin);
		x = EditorGUILayout.Slider(x, 0, 1, options);
		float y = Mathf.Pow(ymax - ymin, x) + ymin;
		Rect position = GUILayoutUtility.GetLastRect();
		position.x = position.x + position.width - 50;
		position.width = 50;
		GUI.Box(position, GUIContent.none);

		GUI.SetNextControlName("realFloatField");
		y = Mathf.Clamp(EditorGUI.FloatField(position, y), ymin, ymax);
		if (position.Contains(Event.current.mousePosition))
		{
			GUI.FocusControl("realFloatField");
		}

		return y;
	}

	public static float ExponentialSlider(GUIContent label, float val, float ymin, float ymax, params GUILayoutOption[] options)
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(label);
		val = ExponentialSlider(val, ymin, ymax);
		EditorGUILayout.EndHorizontal();
		return val;
	}

	public static float ExponentialSlider(Rect r, float val, float ymin, float ymax)
	{
		val = Mathf.Clamp(val, ymin, ymax);
		ymin -= 1; //Because ur face that's why
		//val is "y"
		float x = Mathf.Log(val - ymin) / Mathf.Log(ymax - ymin);
		x = EditorGUI.Slider(r, x, 0, 1);
		float y = Mathf.Pow(ymax - ymin, x) + ymin;
		r.x = r.x + r.width - 50;
		r.width = 50;
		GUI.Box(r, GUIContent.none);

		GUI.SetNextControlName("realFloatField");
		y = Mathf.Clamp(EditorGUI.FloatField(r, y), ymin, ymax);
		if (r.Contains(Event.current.mousePosition))
		{
			GUI.FocusControl("realFloatField");
		}

		return y;
	}

	#endregion

	#region Search Field

	private static MethodInfo searchField
	{
		get { return _searchField ?? (_searchField = typeof(EditorGUI).GetMethod("SearchField", BindingFlags.NonPublic | BindingFlags.Static)); }
	}

	private static MethodInfo _searchField;
	
	public static string SearchField(Rect r, string searchString)
	{
		return (string)searchField.Invoke(null, new object[]{r, searchString});
	}
	
	#endregion


	public static string IPSelectionPopup(string label, string currentIP, params GUILayoutOption[] options){
		Rect r = EditorGUILayout.GetControlRect (options);
		return IPSelectionPopup (new GUIContent(label), r, currentIP);
	}

	public static string IPSelectionPopup(GUIContent label, Rect r, string currentIP) {
		r = EditorGUI.PrefixLabel (r, label);
		string hn = Dns.GetHostName ();
		List<string> ipv4 = new List<string> ();
		foreach(IPAddress ip in Dns.GetHostAddresses(hn)) {
			if(ip.IsIPv6LinkLocal || ip.IsIPv6Multicast || ip.IsIPv6SiteLocal) {
				continue;
			}
			ipv4.Add (ip.ToString ());
		}
		if(ipv4.Count == 0) {
			EditorGUI.LabelField (r, "No valid IP addresses! Check your network adapter.");
			return "";
		}
		ipv4.Sort ((x, y) => x.Length.CompareTo(y.Length));
		int selectedIP = ipv4.IndexOf (currentIP);
		if(selectedIP == -1) {
			selectedIP = 0;
		}
		selectedIP = EditorGUI.Popup (r, selectedIP, ipv4.ToArray());
		return ipv4 [selectedIP];
	}

	public static bool ButtonOverPreviousControl () {
		Rect r = GUILayoutUtility.GetLastRect ();
		return GUI.Button (r, GUIContent.none, GUIStyle.none);
	}

	public static bool ButtonOverPreviousControl (float w) {
		Rect r = GUILayoutUtility.GetLastRect ();
		r.width = w;
		return GUI.Button (r, GUIContent.none, GUIStyle.none);
	}

	public static void DrawIconOverLast (Texture2D t, RectOffset o = null) {
		Color last = GUI.color;
		GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.grey;
		Rect p = GUILayoutUtility.GetLastRect ();
		float h = p.height;
		p.xMin = p.xMax - h;
		p.width = h;
		if(o != null)
			p = o.Add (p);
		GUI.DrawTexture (p, t);
		GUI.color = last;
	}

	public static bool LayoutButtonWithOffset (string label, RectOffset offset, params GUILayoutOption[] options){
		return LayoutButtonWithOffset (new GUIContent(label), offset, options);
	}

	public static bool LayoutButtonWithOffset (GUIContent guiContent, RectOffset offset, params GUILayoutOption[] options){
		return LayoutButtonWithOffset(guiContent, EditorGUIUtility.singleLineHeight, offset, options);
	}

	public static bool LayoutButtonWithOffset (string label, float height, RectOffset offset, params GUILayoutOption[] options){
		return LayoutButtonWithOffset(new GUIContent(label), height, offset, options);
	}

	public static bool LayoutButtonWithOffset (GUIContent guiContent, float height, RectOffset offset, params GUILayoutOption[] options){
		Rect r = EditorGUILayout.GetControlRect (false, height, GUI.skin.button, options);
		r = offset.Add (r);
		return GUI.Button (r, guiContent);
	}

	public static bool LayoutButtonWithOffset (GUIContent guiContent, RectOffset offset, GUIStyle style, params GUILayoutOption[] options){
		return LayoutButtonWithOffset(guiContent, EditorGUIUtility.singleLineHeight, offset, style, options);
	}

	public static bool LayoutButtonWithOffset (GUIContent guiContent, float height, RectOffset offset, GUIStyle style, params GUILayoutOption[] options){
		Rect r = EditorGUILayout.GetControlRect (false, height, GUI.skin.button, options);
		r = offset.Add (r);
		return GUI.Button (r, guiContent, style);
	}

	public static bool DrawPlayModeWarning (string label, MessageType messageType){
		if (!Application.isPlaying) return false;
		EditorGUILayout.HelpBox (string.Format (label), messageType);
		return true;
	}
	
	public static void DrawBetterSelectableLabel (string label, GUIStyle style, params GUILayoutOption[] options){
		GUILayoutOption[] options2 = new GUILayoutOption[options.Length + 1];
		for (int i = 0; i < options.Length; i++) {
			options2 [i] = options [i];
		}
		options2 [options.Length] = GUILayout.Height (EditorGUIUtility.singleLineHeight);
		EditorGUILayout.SelectableLabel(label, style,
			options); //For some reason the selectable label likes to be two lines high
	}
}
