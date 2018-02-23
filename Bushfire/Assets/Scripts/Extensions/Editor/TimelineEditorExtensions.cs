using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace Alchemy
{
    public static class TimelineEditorExtensions {
        #region Duplicate

        private static Type _projectWindowUtilType;
        private static Type projectWindowUtilType {
            get { return _projectWindowUtilType ?? (_projectWindowUtilType = typeof(ProjectWindowUtil)); }
        }
        
        static MethodInfo _duplicateSelectedAssets;
        static MethodInfo duplicateSelectedAssets {
            get
            {
                return _duplicateSelectedAssets ?? (_duplicateSelectedAssets =
                           projectWindowUtilType.GetMethod("DuplicateSelectedAssets",
                               BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
            }
        }
        
        public static TimelineAsset DuplicateTimelineAsset (TimelineAsset asset) {
            /*	//This old function works in most cases, but the Recorder assets saved into some Timelines magically become the MainObject in the asset. Now we need to swap them ):
            Selection.objects = new Object[]{ asset };
            Selection.activeObject = asset;
            duplicateSelectedAssets.Invoke (null, null);
            return (TimelineAsset)Selection.objects [0];
            */
            Selection.objects = new Object[]{ asset };
            Selection.activeObject = asset;
            duplicateSelectedAssets.Invoke (null, null);
            string path = AssetDatabase.GetAssetPath (Selection.objects [0]);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath (path);
            foreach (Object o in assets) {
                TimelineAsset t = o as TimelineAsset;
                if (t == null) continue;
                AssetDatabase.SetMainObject (t, path);
                return t;
            }

            return null;
        }

        #endregion
        
        public static void SetTrackParent (TrackAsset track, PlayableAsset parent){
            SerializedObject trackSO = new SerializedObject(track);
            SerializedProperty parentProp = trackSO.FindProperty("m_Parent");
            parentProp.objectReferenceValue = parent;
            trackSO.ApplyModifiedPropertiesWithoutUndo();
        }
        
        public static void AddTrackToTimelineAsset (TimelineAsset asset, TrackAsset track, GroupTrack parent){
            MethodInfo addTrackMethod = typeof(TimelineAsset).GetMethod ("AddTrackInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            // ReSharper disable once PossibleNullReferenceException as if this is no longer a method I want to know about it!
            addTrackMethod.Invoke (asset, new object[]{ track });
            if(parent != null)
                track.SetGroup (parent);
            RefreshTimelineState ();
        }
    
        //----------------------------
        private const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static Type _timelineWindowType;
        private static Type timelineWindowType {
            get
            {
                return _timelineWindowType ?? (_timelineWindowType =
                           Type.GetType("UnityEditor.Timeline.TimelineWindow,UnityEditor.Timeline"));
            }
        }
    
        private static Type _timelineWindowStateType;
        private static Type timelineWindowStateType {
            get
            {
                return _timelineWindowStateType ?? (_timelineWindowStateType =
                           Type.GetType("UnityEditor.Timeline.TimelineWindow+TimelineState,UnityEditor.Timeline"));
            }
        }
    
        public static EditorWindow GetTimelineEditorWindow () {
            Object[] allAniWindows = Resources.FindObjectsOfTypeAll (timelineWindowType);
            if (allAniWindows.Length == 0) return null;
            EditorWindow timelineWindow = (EditorWindow)allAniWindows [0];
            return timelineWindow;
        }
    
        static MethodInfo _stateRefresh;
        static MethodInfo stateRefresh {
            get
            {
                return _stateRefresh ??
                       (_stateRefresh = timelineWindowStateType.GetMethod("Refresh", bindingFlags));
            }
        }

        static PropertyInfo _state;
        static PropertyInfo state {
            get { return _state ?? (_state = timelineWindowType.GetProperty("state", bindingFlags)); }
        }

        public static void RefreshTimelineState () {
            stateRefresh.Invoke (state.GetValue (GetTimelineEditorWindow (), null), null);
        }
        
        static FieldInfo _currentDirector;
        static FieldInfo currentDirector {
            get {
                if (_currentDirector == null)
                    _currentDirector = timelineWindowStateType.GetField ("m_CurrentDirector", bindingFlags);
                return _currentDirector;
            }
        }
        
        public static object GetTimelineEditorStateInstance (EditorWindow timelineEditorWindow) {
            if (timelineEditorWindow == null)
            {
                return EditorApplication.ExecuteMenuItem ("Window/Timeline") ? GetTimelineEditorStateInstance (GetTimelineEditorWindow()) : null;
            }
            PropertyInfo stateTimelineWindow = timelineWindowType.GetProperty ("state", bindingFlags);
            // ReSharper disable once PossibleNullReferenceException as if this is no longer a method I want to know about it!
            return stateTimelineWindow.GetValue (timelineEditorWindow, new object[] { });
        }

        public static PlayableDirector GetTimelineEditorWindowPlayableDirector (EditorWindow timelineEditorWindow) {
            object s = GetTimelineEditorStateInstance (timelineEditorWindow);
            return (PlayableDirector)currentDirector.GetValue(s);
        }

        //----------------------------
        
        /// <summary>
        /// Must be called from OnGUI
        /// </summary>
        public static Color GetTrackColor(Type trackAssetType)
        {
            object[] o = trackAssetType.GetCustomAttributes (typeof(TrackColorAttribute), true);
            if(o.Length > 0)
                return ((TrackColorAttribute)o [0]).color;
            Type tDType = GetCustomDrawer.Invoke(null, new object[]{trackAssetType}) as Type;
            if (tDType == null)
                return Color.grey;
            object tD = Activator.CreateInstance(tDType);
            return (Color) trackColor.GetValue(tD, null);
            
        }
        
        public static GUIContent GetTrackIcon(Type trackAssetType)
        {
            Type tDType = GetCustomDrawer.Invoke(null, new object[]{trackAssetType}) as Type;
            if (tDType == null)
                return GUIContent.none;
            object tD = Activator.CreateInstance(tDType);
            //There's a null reference exception if we get the default icon, so return none!
            if(tD.GetType() == trackDrawerType)
                return  GUIContent.none;
            return (GUIContent) GetIcon.Invoke(tD, null);
        }

        private static Type _timelineHelpersType;
        private static Type timelineHelpersType
        {
            get { return _timelineHelpersType ?? (_timelineHelpersType = Type.GetType("UnityEditor.Timeline.TimelineHelpers,UnityEditor.Timeline")); }
        }

        static MethodInfo _GetCustomDrawer;
        static MethodInfo GetCustomDrawer
        {
            get { return _GetCustomDrawer ?? (_GetCustomDrawer = timelineHelpersType.GetMethod("GetCustomDrawer", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)); }
        }

        static Type _trackDrawerType;
        static Type trackDrawerType {
            get { return _trackDrawerType ?? (_trackDrawerType = Type.GetType("UnityEditor.Timeline.TrackDrawer,UnityEditor.Timeline")); }
        }

        static MethodInfo _createTrackDrawerInstance;
        static MethodInfo createTrackDrawerInstance {
            get { return _createTrackDrawerInstance ?? (_createTrackDrawerInstance = trackDrawerType.GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)); }
        }

        static PropertyInfo _trackColor;
        static PropertyInfo trackColor {
            get { return _trackColor ?? (_trackColor = trackDrawerType.GetProperty("trackColor", bindingFlags)); }
        }
        
        static MethodInfo _GetIcon;
        static MethodInfo GetIcon
        {
            get { return _GetIcon ?? (_GetIcon = trackDrawerType.GetMethod("GetIcon", BindingFlags.Public | BindingFlags.Instance)); }
        }
        //----------------------------
        
        static Type _timelineWindowViewPrefsType;
        static Type timelineWindowViewPrefsType {
            get { return _timelineWindowViewPrefsType ?? (_timelineWindowViewPrefsType = Type.GetType("UnityEditor.Timeline.TimelineWindowViewPrefs,UnityEditor.Timeline")); }
        }
        
        static MethodInfo _isTrackCollapsed;
        static MethodInfo isTrackCollapsed {
            get { return _isTrackCollapsed ?? (_isTrackCollapsed = timelineWindowViewPrefsType.GetMethod("IsTrackCollapsed", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)); }
        }

        public static bool IsTrackCollapsed (TrackAsset track) {
            return (bool)isTrackCollapsed.Invoke (null, new object[]{ track });
        }
        
        static MethodInfo _setTrackCollapsed;
        static MethodInfo setTrackCollapsed {
            get { return _setTrackCollapsed ?? (_setTrackCollapsed = timelineWindowViewPrefsType.GetMethod("SetTrackCollapsed", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)); }
        }

        public static void SetTrackCollapsed (TrackAsset track, bool collapsed) {
            PushUndo (track, collapsed);
            setTrackCollapsed.Invoke (null, new object[]{ track, collapsed });
        }

        //----------------------------
        
        static Type _timelineUndoType;
        static Type timelineUndoType {
            get { return _timelineUndoType ?? (_timelineUndoType = Type.GetType("UnityEngine.Timeline.TimelineUndo,UnityEngine.Timeline")); }
        }

        static MethodInfo _pushUndo;
        static MethodInfo pushUndo {
            get { return _pushUndo ?? (_pushUndo = timelineUndoType.GetMethod("PushUndo", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)); }
        }

        static void PushUndo (TrackAsset track, bool collapsed) {
            pushUndo.Invoke (null, new object[]{ track, collapsed ? "Expand Group" : "Collapse Group"});
        }

        //----------------------------
        
        public static void CleanupTimelineAsset (TimelineAsset tA){
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath (AssetDatabase.GetAssetPath (tA));
            bool needsSave = false;
            List<AnimationClip> allAnimationClips = new List<AnimationClip> ();
            GetAllAnimationClipsRecursive (tA.GetRootTracks (), allAnimationClips);
            foreach (Object asset in assets) {
                AnimationClip animClip = asset as AnimationClip;
                if (animClip == null || AssetDatabase.IsMainAsset(asset)) continue;
                if (allAnimationClips.Contains(animClip)) continue;
                Object.DestroyImmediate (asset, true);
                needsSave = true;
            }
            if(needsSave)
                AssetDatabase.SaveAssets();
        }

        public static void GetAllAnimationClipsRecursive (IEnumerable<TrackAsset> tracks, List<AnimationClip> allAnimationClips){
            foreach(TrackAsset track in tracks){
                GroupTrack groupTrack = track as GroupTrack;
                if (groupTrack != null) {
                    GetAllAnimationClipsRecursive (groupTrack.GetChildTracks (), allAnimationClips);
                } else {
                    AnimationClip infiniteAnimationClip = TimelineExtensions.GetAnimClipFromInfiniteOnly (track);
                    if(infiniteAnimationClip != null)
                        allAnimationClips.Add (infiniteAnimationClip);

                    IEnumerable<TimelineClip> timelineClips = track.GetClips ();
                    foreach(TimelineClip timelineClip in timelineClips){
                        AnimationClip animationClip = timelineClip.animationClip;
                        if(animationClip != null)
                            allAnimationClips.Add (animationClip);
                    }
                }
            }
        }
    }
}