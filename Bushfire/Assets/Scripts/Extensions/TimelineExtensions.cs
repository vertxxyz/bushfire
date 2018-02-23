using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alchemy
{
	public static class TimelineExtensions
	{
		public static void ClearTrack (TimelineAsset timelineAsset, TrackAsset trackAsset){
			IEnumerable<TimelineClip> cS = trackAsset.GetClips ();
			#if UNITY_EDITOR
			Undo.RecordObject (timelineAsset, "Set Clean TimelineClip");
			#endif
			foreach (TimelineClip c in cS) {
				timelineAsset.DeleteClip (c);
			}
		}

		public static void RemoveTrackInTimelineAsset(TimelineAsset asset, TrackAsset trackToRemove)
		{
			MethodInfo removeTrackMethod =
				typeof(TimelineAsset).GetMethod("RemoveTrack", BindingFlags.NonPublic | BindingFlags.Instance);
			// ReSharper disable once PossibleNullReferenceException as if this is no longer a method I want to know about it!
			removeTrackMethod.Invoke(asset, new object[] {trackToRemove});
			EditorUtility.SetDirty(asset);
		}

		public static void RemoveAllTracksOfType(TimelineAsset timeline, Type type)
		{
			List<TrackAsset> tracksToRemove = new List<TrackAsset>();
			RunOnAllTracks(timeline, t =>
			{
				if (t.GetType() == type)
					tracksToRemove.Add(t);
			}, false);
			foreach (TrackAsset trackToRemove in tracksToRemove)
			{
				RemoveTrackInTimelineAsset(timeline, trackToRemove);
			}
		}
		
		public static void RunOnRootTracks (TimelineAsset timelineAsset, Action<TrackAsset> @Action){
			IEnumerable<TrackAsset> tracks = timelineAsset.GetRootTracks ();
			foreach (TrackAsset track in tracks) {
				@Action.Invoke (track);
			}
		}

		public static void RunOnAllTracks(TimelineAsset timelineAsset, Action<TrackAsset> Action, bool runOnGroupTracks)
		{
			RunOnTrackIEnumerable(timelineAsset.GetRootTracks(), Action, runOnGroupTracks);
		}

		public static void RunOnTrackIEnumerable(IEnumerable<TrackAsset> tracks, Action<TrackAsset> Action,
			bool runOnGroupTracks)
		{
			foreach (TrackAsset track in tracks)
			{
				GroupTrack groupTrack = track as GroupTrack;
				if (groupTrack != null)
				{
					if (runOnGroupTracks)
						Action.Invoke(track);
					RunOnTrackIEnumerable(groupTrack.GetChildTracks(), Action, runOnGroupTracks);
				}
				else
				{
					Action.Invoke(track);
				}
			}
		}
		
		public static T RunOnAllTracksReturn <T> (TimelineAsset timelineAsset, Func<TrackAsset, object> @Func, bool runOnGroupTracks){
			return (T)RunOnTrackIEnumerableReturn (timelineAsset.GetRootTracks (), @Func, runOnGroupTracks);
		}

		private static object RunOnTrackIEnumerableReturn (IEnumerable<TrackAsset> tracks, Func<TrackAsset, object> @Func, bool runOnGroupTracks){
			foreach (TrackAsset track in tracks) {
				GroupTrack groupTrack = track as GroupTrack;
				object o;
				if (groupTrack != null) {
					if (runOnGroupTracks) {
						o = @Func.Invoke (track);
						if (o != null)
							return o;
					}
					o = RunOnTrackIEnumerableReturn (groupTrack.GetChildTracks (), Func, runOnGroupTracks);
					if (o != null)
						return o;
				} else {
					o = @Func.Invoke (track);
					if (o != null)
						return o;
				}
			}
			return null;
		}

		public static GroupTrack GetGroupTrackWithName(TimelineAsset timeline, string name)
		{
			IEnumerable<TrackAsset> tracks = timeline.GetRootTracks();
			return tracks.OfType<GroupTrack>().FirstOrDefault(groupTrack => groupTrack.name.Equals(name));
		}

		public static GameObject GetGameObjectFromObject(Object o)
		{
			GameObject g = o as GameObject;
			if (g != null)
				return g;
			Component c = o as Component;
			return c != null ? c.gameObject : null;
		}

		//This should use the binding type for the track instead of just binding a GameObject
		public static void SetGenericBinding(PlayableDirector timelineDirector, TrackAsset trackAsset, GameObject gameObject)
		{
			Type bindingType = trackAsset.outputs.First().sourceBindingType;
			if (bindingType == null || bindingType == typeof(GameObject))
				timelineDirector.SetGenericBinding(trackAsset, gameObject);
			else
			{
				Component c = gameObject.GetComponent(bindingType);
				if (c != null)
					timelineDirector.SetGenericBinding(trackAsset, c);
				else
					Debug.LogWarning("Binding type was not found on object " + gameObject);
			}
		}

		public static TimelineClip SetAnimationClipAndCreateTrackClip(AnimationTrack t, AnimationClip aC)
		{
#if UNITY_EDITOR
			Undo.RecordObject(t, "Set Clean TimelineClip");
#endif
			TimelineClip tC = t.CreateClip(aC);
			tC.duration = aC.length;
			return tC;
		}

		static FieldInfo _m_AnimClip;
		static FieldInfo m_AnimClip {
			get { return _m_AnimClip ?? (_m_AnimClip = typeof(TrackAsset).GetField("m_AnimClip", bindingFlags)); }
		}
		public static AnimationClip GetAnimClip(TrackAsset t) {
			AnimationClip clip = GetAnimClipFromInfiniteOnly (t);
			if (clip != null)
				return clip;
			TimelineClip tC;
			return GetAnimationClipFromFirstClipOnly (t, out tC);
		}

		public static AnimationClip GetAnimClipFromInfiniteOnly (TrackAsset t){
			AnimationClip clip = (AnimationClip)m_AnimClip.GetValue (t);
			return clip;
		}
		
		public static AnimationClip GetAnimationClipFromFirstClipOnly(TrackAsset t, out TimelineClip tC)
		{
			tC = GetFirstClip(t);
			return tC == null ? null : tC.animationClip;
		}

		public static TimelineClip GetFirstClip(TrackAsset t)
		{
			IEnumerable<TimelineClip> cS = t.GetClips();
			var timelineClips = cS.ToList();
			return timelineClips.Count == 0 ? null : timelineClips.First();
		}

		//-------------------------------------------------------------------------

		static PropertyInfo _isLocked;

		static PropertyInfo isLocked
		{
			get
			{
				return _isLocked ??
				       (_isLocked = typeof(TrackAsset).GetProperty("locked", BindingFlags.NonPublic | BindingFlags.Instance));
			}
		}

		public static bool GetTrackLocked(TrackAsset trackAsset)
		{
			return (bool) isLocked.GetValue(trackAsset, null);
		}

		public static void SetTrackLocked(TrackAsset trackAsset, bool locked)
		{
			isLocked.SetValue(trackAsset, locked, null);
		}

		//-------------------------------------------------------------------------

#if UNITY_EDITOR
		private const BindingFlags bindingFlags =
			BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public static EditorWindow GetTimelineEditorWindow()
		{
			Object[] allAniWindows = Resources.FindObjectsOfTypeAll(timelineWindowType);
			if (allAniWindows.Length == 0) return null;
			EditorWindow timelineWindow = (EditorWindow) allAniWindows[0];
			return timelineWindow;
		}

		private static Type _timelineWindowType;

		private static Type timelineWindowType
		{
			get
			{
				return _timelineWindowType ?? (_timelineWindowType =
					       Type.GetType("UnityEditor.Timeline.TimelineWindow,UnityEditor.Timeline"));
			}
		}

		private static Type _timelineWindowStateType;

		private static Type timelineWindowStateType
		{
			get
			{
				return _timelineWindowStateType ?? (_timelineWindowStateType =
					       Type.GetType("UnityEditor.Timeline.TimelineWindow+TimelineState,UnityEditor.Timeline"));
			}
		}

		static FieldInfo _currentDirector;

		static FieldInfo currentDirector
		{
			get
			{
				return _currentDirector ??
				       (_currentDirector = timelineWindowStateType.GetField("m_CurrentDirector", bindingFlags));
			}
		}

		private static object GetTimelineEditorStateInstance(EditorWindow timelineEditorWindow)
		{
			if (timelineEditorWindow == null)
			{
				return EditorApplication.ExecuteMenuItem("Window/Timeline")
					? GetTimelineEditorStateInstance(GetTimelineEditorWindow())
					: null;
			}

			PropertyInfo stateTimelineWindow = timelineWindowType.GetProperty("state", bindingFlags);
			// ReSharper disable once PossibleNullReferenceException as if this is no longer a method I want to know about it!
			return stateTimelineWindow.GetValue(timelineEditorWindow, new object[] { });
		}

		public static PlayableDirector GetTimelineEditorWindowPlayableDirector(EditorWindow timelineEditorWindow)
		{
			object s = GetTimelineEditorStateInstance(timelineEditorWindow);
			return (PlayableDirector) currentDirector.GetValue(s);
		}
		#endif

		static MethodInfo _addClip;
		static MethodInfo addClip {
			get { return _addClip ?? (_addClip = typeof(TrackAsset).GetMethod("AddClip", BindingFlags.NonPublic | BindingFlags.Instance)); }
		}

		public static void AddClipToTrack (TrackAsset track, TimelineClip clip){
			addClip.Invoke (track, new object[]{ clip });
		}
		
		static MethodInfo _assignAnimationClip;
		static MethodInfo assignAnimationClip {
			get { return _assignAnimationClip ?? (_assignAnimationClip = typeof(AnimationTrack).GetMethod("AssignAnimationClip", bindingFlags)); }
		}
		public static void AssignAnimationClipToTimelineClip (AnimationTrack track, TimelineClip clip, AnimationClip aC){
			assignAnimationClip.Invoke (track, new object[]{ clip, aC });
		}

		static FieldInfo _m_Clips;
		static FieldInfo m_Clips {
			get { return _m_Clips ?? (_m_Clips = typeof(TrackAsset).GetField("m_Clips", bindingFlags)); }
		}

		static FieldInfo _m_ClipsCache;
		static FieldInfo m_ClipsCache {
			get { return _m_ClipsCache ?? (_m_ClipsCache = typeof(TrackAsset).GetField("m_ClipsCache", bindingFlags)); }
		}

		public static void SetTimelineClips (TrackAsset t, List<TimelineClip> timelineClips){
			m_Clips.SetValue (t, timelineClips);
			m_ClipsCache.SetValue (t, null);
		}
		
		public static void SetAnimClipInfinite(TrackAsset t, AnimationClip aC) {
			m_AnimClip.SetValue (t, aC);
		}

	}
}