﻿using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace Alchemy
{
	public class BuiltInResourcesWindow : EditorWindow
	{
		[MenuItem( "Window/Built-in styles and icons" )]
		public static void ShowWindow ()
		{
			BuiltInResourcesWindow w = GetWindow<BuiltInResourcesWindow> ();
			w.Show ();
		}

		private struct Drawing
		{
			public Rect Rect;
			public Action Draw;
		}

		private List<Drawing> Drawings;
	
		private List<UnityEngine.Object> _objects;
		private float _scrollPos;
		private float _maxY;
		private Rect _oldPosition;
	
		private bool _showingStyles = true;
		private bool _showingIcons = false;
		private bool _showingOtherIcons = false;
	
		private string _search = "";

		public void OnGUI ()
		{
			if (position.width != _oldPosition.width && Event.current.type == EventType.Layout) {
				Drawings = null;
				_oldPosition = position;
			}
	
			GUILayout.BeginHorizontal ();
	
			if(GUILayout.Button ("Styles", EditorStyles.toolbarButton)) {
				_showingIcons = true;
				_showingStyles = false;
				_showingOtherIcons = false;
				Drawings = null;
			}
	
			if(GUILayout.Button ("Icons", EditorStyles.toolbarButton)) {
				_showingIcons = false;
				_showingStyles = true;
				_showingOtherIcons = false;
				Drawings = null;
			}

			if(GUILayout.Button ("Other Icons", EditorStyles.toolbarButton)) {
				_showingIcons = false;
				_showingStyles = false;
				_showingOtherIcons = true;
				Drawings = null;
			}
	
			GUILayout.EndHorizontal ();
	
			string newSearch = GUILayout.TextField (_search);
			if (newSearch != _search) {
				_search = newSearch;
				Drawings = null;
			}

			if (_showingOtherIcons) {
				DrawAllIcons ();
				return;
			}
	
			float top = 36;
	
			if (Drawings == null) {
				string lowerSearch = _search.ToLower ();
	
				Drawings = new List<Drawing> ();
	
				GUIContent inactiveText = new GUIContent ("inactive");
				GUIContent activeText = new GUIContent ("active");
	
				float x = 5.0f;
				float y = 5.0f;
	
				if (_showingStyles) {
					foreach (GUIStyle ss in GUI.skin.customStyles) {
						if (lowerSearch != "" && !ss.name.ToLower ().Contains (lowerSearch))
							continue;
	
						GUIStyle thisStyle = ss;
	
						Drawing draw = new Drawing ();
	
						float width = Mathf.Max (
							              100.0f,
							              GUI.skin.button.CalcSize (new GUIContent (ss.name)).x,
							              ss.CalcSize (inactiveText).x + ss.CalcSize (activeText).x
						              ) + 16.0f;
	
						float height = 60.0f;
	
						if (x + width > position.width - 32 && x > 5.0f) {
							x = 5.0f;
							y += height + 10.0f;
						}
	
						draw.Rect = new Rect (x, y, width, height);
	
						width -= 8.0f;
	
						draw.Draw = () => {
							if (GUILayout.Button (thisStyle.name, GUILayout.Width (width)))
								CopyText ("(GUIStyle)\"" + thisStyle.name + "\"");
	
							GUILayout.BeginHorizontal ();
							GUILayout.Toggle (false, inactiveText, thisStyle, GUILayout.Width (width / 2));
							GUILayout.Toggle (false, activeText, thisStyle, GUILayout.Width (width / 2));
							GUILayout.EndHorizontal ();
						};
	
						x += width + 18.0f;
	
						Drawings.Add (draw);
					}
				} else if (_showingIcons) {
					if (_objects == null) {
						_objects = new List<UnityEngine.Object> (Resources.FindObjectsOfTypeAll (typeof(Texture)));
						_objects.Sort (( pA, pB) => System.String.Compare (pA.name, pB.name, System.StringComparison.OrdinalIgnoreCase));
					}
	
					float rowHeight = 0.0f;
	
					foreach (UnityEngine.Object oo in _objects) {
						Texture texture = (Texture)oo;
						if (texture != null) {
							if (texture.name == "")
								continue;
		
							if (lowerSearch != "" && !texture.name.ToLower ().Contains (lowerSearch))
								continue;
							if (texture is Cubemap || texture is RenderTexture || texture is Texture2DArray || texture is Texture3D || texture is CubemapArray)
								continue;
							Drawing draw = new Drawing ();

	
							float width = Mathf.Max (
								             GUI.skin.button.CalcSize (new GUIContent (texture.name)).x,
								             texture.width
							             ) + 8.0f;
	
							float height = texture.height + GUI.skin.button.CalcSize (new GUIContent (texture.name)).y + 8.0f;
	
							if (x + width > position.width - 32.0f) {
								x = 5.0f;
								y += rowHeight + 8.0f;
								rowHeight = 0.0f;
							}
	
							draw.Rect = new Rect (x, y, width, height);
	
							rowHeight = Mathf.Max (rowHeight, height);
	
							width -= 8.0f;
	
							draw.Draw = () => {
								if (GUILayout.Button (texture.name, GUILayout.Width (width)))
									CopyText ("EditorGUIUtility.FindTexture( \"" + texture.name + "\" )");
	
								Rect textureRect = GUILayoutUtility.GetRect (texture.width, texture.width, texture.height, texture.height, GUILayout.ExpandHeight (false), GUILayout.ExpandWidth (false));
								EditorGUI.DrawTextureTransparent (textureRect, texture);
							};
	
							x += width + 8.0f;
	
							Drawings.Add (draw);
						}
					}
				}
	
				_maxY = y;
			}
	
			Rect r = position;
			r.y = top;
			r.height -= r.y;
			r.x = r.width - 16;
			r.width = 16;
	
			float areaHeight = position.height - top;
			_scrollPos = GUI.VerticalScrollbar (r, _scrollPos, areaHeight, 0.0f, _maxY);
	
			Rect area = new Rect (0, top, position.width - 16.0f, areaHeight);
			GUILayout.BeginArea (area);
	
			int count = 0;
			foreach (Drawing draw in Drawings) {
				Rect newRect = draw.Rect;
				newRect.y -= _scrollPos;
	
				if (newRect.y + newRect.height > 0 && newRect.y < areaHeight) {
					GUILayout.BeginArea (newRect, GUI.skin.textField);
					draw.Draw ();
					GUILayout.EndArea ();
	
					count++;
				}
			}
	
			GUILayout.EndArea ();
		}

		void CopyText (string pText)
		{
			TextEditor editor = new TextEditor ();
	
			//editor.content = new GUIContent(pText); // Unity 4.x code
			editor.text = pText; // Unity 5.x code
	
			editor.SelectAll ();
			editor.Copy ();
		}

		public void DrawAllIcons () {
			string searchLower = _search.ToLower ();
			using (EditorGUILayout.ScrollViewScope scroll = new EditorGUILayout.ScrollViewScope (new Vector2(0, _scrollPos))) {
				foreach (string icon in icons) {
					if (icon.ToLower().Contains (searchLower)) {
						if (GUILayout.Button (EditorGUIUtility.IconContent (icon), EditorStyles.helpBox))
							Debug.Log (icon);
					}
				}
				_scrollPos = scroll.scrollPosition.y;
			}
		}

		string[] icons = new string[] {
			"ScriptableObject Icon",
			"_Popup",
			"_Help",
			"Clipboard",
			"SocialNetworks.UDNOpen",
			"SocialNetworks.Tweet",
			"SocialNetworks.FacebookShare",
			"SocialNetworks.LinkedInShare",
			"SocialNetworks.UDNLogo",
			"animationvisibilitytoggleoff",
			"animationvisibilitytoggleon",
			"tree_icon",
			"tree_icon_leaf",
			"tree_icon_frond",
			"tree_icon_branch",
			"tree_icon_branch_frond",
			"editicon.sml",
			"TreeEditor.Refresh",
			"TreeEditor.Duplicate",
			"TreeEditor.Trash",
			"TreeEditor.AddBranches",
			"TreeEditor.AddLeaves",
			"preAudioPlayOn",
			"preAudioPlayOff",
			"AvatarInspector/RightFingersIk",
			"AvatarInspector/LeftFingersIk",
			"AvatarInspector/RightFeetIk",
			"AvatarInspector/LeftFeetIk",
			"AvatarInspector/RightFingers",
			"AvatarInspector/LeftFingers",
			"AvatarInspector/RightArm",
			"AvatarInspector/LeftArm",
			"AvatarInspector/RightLeg",
			"AvatarInspector/LeftLeg",
			"AvatarInspector/Head",
			"AvatarInspector/Torso",
			"AvatarInspector/MaskEditor_Root",
			"AvatarInspector/BodyPartPicker",
			"AvatarInspector/BodySIlhouette",
			"Mirror",
			"SpeedScale",
			"Toolbar Minus",
			"Toolbar Plus More",
			"Toolbar Plus",
			"AnimatorController Icon",
			"TextAsset Icon",
			"Shader Icon",
			"boo Script Icon",
			"cs Script Icon",
			"js Script Icon",
			"Prefab Icon",
			"Profiler.NextFrame",
			"Profiler.PrevFrame",
			"sv_icon_none",
			"ColorPicker.CycleSlider",
			"ColorPicker.CycleColor",
			"EyeDropper.Large",
			"ClothInspector.PaintValue",
			"ClothInspector.ViewValue",
			"ClothInspector.SettingsTool",
			"ClothInspector.PaintTool",
			"ClothInspector.SelectTool",
			"WelcomeScreen.AssetStoreLogo",
			"WelcomeScreen.AssetStoreLogo",
			"AboutWindow.MainHeader",
			"UnityLogo",
			"AgeiaLogo",
			"MonoLogo",
			"Toolbar Minus",
//			"PlayButtonProfile Anim",
//			"StepButton Anim",
//			"PauseButton Anim",
//			"PlayButton Anim",
			"PlayButtonProfile On",
			"StepButton On",
			"PauseButton On",
			"PlayButton On",
			"PlayButtonProfile",
			"StepButton",
			"PauseButton",
			"PlayButton",
			"ViewToolOrbit On",
			"ViewToolZoom On",
			"ViewToolMove On",
			"ViewToolOrbit On",
			"ViewToolOrbit",
			"ViewToolZoom",
			"ViewToolMove",
			"ViewToolOrbit",
			"ScaleTool On",
			"RotateTool On",
			"MoveTool On",
			"ScaleTool",
			"RotateTool",
			"MoveTool",
			"PauseButton",
			"PlayButton",
			"Toolbar Minus",
			"Toolbar Plus",
			"UnityLogo",
			"_Help",
			"_Popup",
			"Icon Dropdown",
			"Avatar Icon",
			"AvatarPivot",
			"SpeedScale",
			"AvatarInspector/DotSelection",
			"AvatarInspector/DotFrameDotted",
			"AvatarInspector/DotFrame",
			"AvatarInspector/DotFill",
			"AvatarInspector/RightHandZoom",
			"AvatarInspector/LeftHandZoom",
			"AvatarInspector/HeadZoom",
			"AvatarInspector/RightLeg",
			"AvatarInspector/LeftLeg",
			"AvatarInspector/RightFingers",
			"AvatarInspector/RightArm",
			"AvatarInspector/LeftFingers",
			"AvatarInspector/LeftArm",
			"AvatarInspector/Head",
			"AvatarInspector/Torso",
			"AvatarInspector/RightHandZoomSilhouette",
			"AvatarInspector/LeftHandZoomSilhouette",
			"AvatarInspector/HeadZoomSilhouette",
			"AvatarInspector/BodySilhouette",
			"Animation.AddKeyframe",
			"Animation.NextKey",
			"Animation.PrevKey",
			"lightMeter/redLight",
			"lightMeter/orangeLight",
			"lightMeter/lightRim",
			"lightMeter/greenLight",
			"Animation.AddEvent",
			"SceneviewAudio",
			"SceneviewLighting",
			"MeshRenderer Icon",
			"Terrain Icon",
			"sv_icon_none",
			"BuildSettings.SelectedIcon",
			"Animation.AddEvent",
			"Animation.AddKeyframe",
			"Animation.NextKey",
			"Animation.PrevKey",
			"Animation.Record",
			"Animation.Play",
			"PreTextureRGB",
			"PreTextureAlpha",
			"PreTextureMipMapHigh",
			"PreTextureMipMapLow",
			"TerrainInspector.TerrainToolSettings",
			"TerrainInspector.TerrainToolPlants",
			"TerrainInspector.TerrainToolTrees",
			"TerrainInspector.TerrainToolSplat",
			"TerrainInspector.TerrainToolSmoothHeight",
			"TerrainInspector.TerrainToolSetHeight",
			"TerrainInspector.TerrainToolRaise",
			"SettingsIcon",
			"PauseButton",
			"PlayButton",
			"PreMatLight1",
			"PreMatLight0",
			"PreMatTorus",
			"PreMatCylinder",
			"PreMatCube",
			"PreMatSphere",
			"PreMatLight1",
			"PreMatLight0",
			"Camera Icon",
			"Toolbar Minus",
			"Toolbar Plus",
			"Animation.EventMarker",
			"AS Badge New",
//			"AS Badge Move",
			"AS Badge Delete",
			"WaitSpin00",
			"WaitSpin01",
			"WaitSpin02",
			"WaitSpin03",
			"WaitSpin04",
			"WaitSpin05",
			"WaitSpin06",
			"WaitSpin07",
			"WaitSpin08",
			"WaitSpin09",
			"WaitSpin10",
			"WaitSpin11",
//			"WelcomeScreen.AssetStoreLogo",
//			"WelcomeScreen.UnityAnswersLogo",
//			"WelcomeScreen.UnityForumLogo",
//			"WelcomeScreen.UnityBasicsLogo",
//			"WelcomeScreen.VideoTutLogo",
//			"WelcomeScreen.MainHeader",
			"UnityLogo",
			"preAudioPlayOn",
			"preAudioPlayOff",
			"Animation.EventMarker",
			"PreMatLight1",
			"PreMatLight0",
			"PreMatTorus",
			"PreMatCylinder",
			"PreMatCube",
			"PreMatSphere",
			"TreeEditor.Duplicate",
			"Toolbar Minus",
			"Toolbar Plus",
			"PreTextureMipMapHigh",
			"PreTextureMipMapLow",
			"PreTextureRGB",
			"PreTextureAlpha",
			"VerticalSplit",
			"HorizontalSplit",
			"Icon Dropdown",
			"PrefabNormal Icon",
			"PrefabModel Icon",
			"PrefabNormal Icon",
			"Prefab Icon",
			"GameObject Icon",
			"preAudioLoopOn",
			"preAudioLoopOff",
			"preAudioPlayOn",
			"preAudioPlayOff",
			"preAudioAutoPlayOn",
			"preAudioAutoPlayOff",
			"BuildSettings.Web.Small",
			"BuildSettings.Standalone.Small",
			"BuildSettings.iPhone.Small",
			"BuildSettings.Android.Small",
			"BuildSettings.BlackBerry.Small",
			"BuildSettings.Tizen.Small",
			"BuildSettings.XBox360.Small",
			"BuildSettings.XboxOne.Small",
//			"BuildSettings.PS3.Small",
			"BuildSettings.PSP2.Small",
			"BuildSettings.PS4.Small",
			"BuildSettings.PSM.Small",
			"BuildSettings.FlashPlayer.Small",
			"BuildSettings.Metro.Small",
			"BuildSettings.WP8.Small",
			"BuildSettings.SamsungTV.Small",
			"BuildSettings.Web",
			"BuildSettings.Standalone",
			"BuildSettings.iPhone",
			"BuildSettings.Android",
			"BuildSettings.BlackBerry",
			"BuildSettings.Tizen",
			"BuildSettings.XBox360",
			"BuildSettings.XboxOne",
//			"BuildSettings.PS3",
			"BuildSettings.PSP2",
			"BuildSettings.PS4",
			"BuildSettings.PSM",
			"BuildSettings.FlashPlayer",
			"BuildSettings.Metro",
			"BuildSettings.WP8",
			"BuildSettings.SamsungTV",
			"TreeEditor.BranchTranslate",
			"TreeEditor.BranchRotate",
			"TreeEditor.BranchFreeHand",
			"TreeEditor.BranchTranslate On",
			"TreeEditor.BranchRotate On",
			"TreeEditor.BranchFreeHand On",
			"TreeEditor.LeafTranslate",
			"TreeEditor.LeafRotate",
			"TreeEditor.LeafTranslate On",
			"TreeEditor.LeafRotate On",
			"sv_icon_dot15_pix16_gizmo",
			"sv_icon_dot1_sml",
			"sv_icon_dot4_sml",
			"sv_icon_dot7_sml",
			"sv_icon_dot5_pix16_gizmo",
			"sv_icon_dot11_pix16_gizmo",
			"sv_icon_dot12_sml",
			"sv_icon_dot15_sml",
			"sv_icon_dot9_pix16_gizmo",
			"sv_icon_name6",
			"sv_icon_name3",
			"sv_icon_name4",
			"sv_icon_name0",
			"sv_icon_name1",
			"sv_icon_name2",
			"sv_icon_name5",
			"sv_icon_name7",
			"sv_icon_dot1_pix16_gizmo",
			"sv_icon_dot8_pix16_gizmo",
			"sv_icon_dot2_pix16_gizmo",
			"sv_icon_dot6_pix16_gizmo",
			"sv_icon_dot0_sml",
			"sv_icon_dot3_sml",
			"sv_icon_dot6_sml",
			"sv_icon_dot9_sml",
			"sv_icon_dot11_sml",
			"sv_icon_dot14_sml",
			"sv_label_0",
			"sv_label_1",
			"sv_label_2",
			"sv_label_3",
			"sv_label_5",
			"sv_label_6",
			"sv_label_7",
			"sv_icon_none",
			"sv_icon_dot14_pix16_gizmo",
			"sv_icon_dot7_pix16_gizmo",
			"sv_icon_dot3_pix16_gizmo",
			"sv_icon_dot0_pix16_gizmo",
			"sv_icon_dot2_sml",
			"sv_icon_dot5_sml",
			"sv_icon_dot8_sml",
			"sv_icon_dot10_pix16_gizmo",
			"sv_icon_dot12_pix16_gizmo",
			"sv_icon_dot10_sml",
			"sv_icon_dot13_sml",
			"sv_icon_dot4_pix16_gizmo",
			"sv_label_4",
			"sv_icon_dot13_pix16_gizmo"
		};
	}
}