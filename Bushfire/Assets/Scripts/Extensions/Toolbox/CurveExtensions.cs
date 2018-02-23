/*Thomas Ingram 2016*/
using UnityEngine;
using System.Collections;

public static class CurveExtensions {
	public static AnimationCurve Lerp (this AnimationCurve curve1, AnimationCurve curve2, float t){
		AnimationCurve curve1_ToLerp = new AnimationCurve(curve1.keys);
		AnimationCurve curve2_ToLerp = new AnimationCurve(curve2.keys);
		for(int i = 0; i<curve2.keys.Length; i++){
			bool found = false;
			for(int j = 0; j<curve1_ToLerp.length; j++){
				if(curve2.keys[i].time == curve1_ToLerp.keys[j].time){
					found = true;
				}
			}
			if(!found){
				curve1_ToLerp.AddKey(curve2.keys[i].time, curve1.Evaluate(curve2.keys[i].time));
			}
		}

		for(int i = 0; i<curve1.keys.Length; i++){
			bool found = false;
			for(int j = 0; j<curve2_ToLerp.length; j++){
				if(curve1.keys[i].time == curve2_ToLerp.keys[j].time){
					found = true;
				}
			}
			if(!found){
				curve2_ToLerp.AddKey(curve1.keys[i].time, curve2.Evaluate(curve1.keys[i].time));
			}
		}

		AnimationCurve newCurve = new AnimationCurve(curve1_ToLerp.keys);
		for(int i = 0; i<newCurve.keys.Length; i++){
			newCurve.MoveKey(i, curve1_ToLerp[i].Lerp(curve2_ToLerp[i], t));
		}
		return newCurve;
	}

	public static void LerpWith (this AnimationCurve curveWith, AnimationCurve curve1, AnimationCurve curve2, float t){
		curveWith = new AnimationCurve(curve1.keys);
		AnimationCurve curve2_ToLerp = new AnimationCurve(curve2.keys);
		for(int i = 0; i<curve2.keys.Length; i++){
			bool found = false;
			for(int j = 0; j<curveWith.length; j++){
				if(curve2.keys[i].time == curveWith.keys[j].time){
					found = true;
				}
			}
			if(!found){
				curveWith.AddKey(curve2.keys[i].time, curve1.Evaluate(curve2.keys[i].time));
			}
		}

		for(int i = 0; i<curve1.keys.Length; i++){
			bool found = false;
			for(int j = 0; j<curve2_ToLerp.length; j++){
				if(curve1.keys[i].time == curve2_ToLerp.keys[j].time){
					found = true;
				}
			}
			if(!found){
				curve2_ToLerp.AddKey(curve1.keys[i].time, curve2.Evaluate(curve1.keys[i].time));
			}
		}

		for(int i = 0; i<curveWith.keys.Length; i++){
			curveWith.MoveKey(i, curveWith[i].Lerp(curve2_ToLerp[i], t));
		}
	}

	public static Keyframe Lerp (this Keyframe key1, Keyframe key2, float t){
		return new Keyframe(Mathf.Lerp(key1.time,key2.time,t),
			Mathf.Lerp(key1.value,key2.value,t),
			Mathf.Lerp(key1.inTangent,key2.inTangent,t),
			Mathf.Lerp(key1.outTangent,key2.outTangent,t));
	}
}