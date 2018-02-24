using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ToggleAndPlayClip : MonoBehaviour
{
	private void Start()
	{
		_animation = GetComponent<Animation>();
	}

	private Animation _animation;
	public AnimationClip animationClipA;
	public AnimationClip animationClipB;
	public bool dontToggleWhilstPlaying = true;
	
	public void ToggleAndPlay()
	{
		if (dontToggleWhilstPlaying)
		{
			if(_animation.isPlaying)
				return;
		}
		_animation.clip = _animation.clip == animationClipA ? animationClipB : animationClipA;
		_animation.Play();
	}

	public void PlayAIfB()
	{
		if(_animation.clip != animationClipB)
			return;
		_animation.clip = animationClipA;
		_animation.Play();
	}
	
	public void PlayBIfA()
	{
		if(_animation.clip != animationClipA)
			return;
		_animation.clip = animationClipB;
		_animation.Play();
	}
}
