using UnityEngine;
using UnityEngine.Events;

public class RadioButtonManager : MonoBehaviour
{
	public AnimationClip pressIn;
	public AnimationClip popOut;

	[Header("Automatically adds Animation to children"), ReadOnly]
	public Transform[] children;
	private Animation[] _animations;

	public UnityEvent[] events;
	public UnityEvent anyEvent;

	private void Reset()
	{
		children = new Transform[transform.childCount];
		for (int i = 0; i<transform.childCount; i++)
		{
			children[i] = transform.GetChild(i);
		}
	}

	public int indexStartedPressed = 0;

	private void Start()
	{
		Reset();
		_animations = new Animation[children.Length];
		for (var i = 0; i < children.Length; i++)
		{
			var child = children[i];
			_animations[i] = child.gameObject.AddComponent<Animation>();
			_animations[i].AddClip(pressIn, pressIn.name);
			_animations[i].AddClip(popOut, popOut.name);
			_animations[i].playAutomatically = false;
		}
		PressIndex(indexStartedPressed, false);
	}

	public void PressIndex(int index)
	{
		// ReSharper disable once IntroduceOptionalParameters.Global
		// Because we want to use this Method in UnityEvents
		PressIndex(index, true);
	}
	
	public void PressIndex(int index, bool playAudio)
	{
		for (int i = 0; i < _animations.Length; i++)
		{
			if (index == i)
			{
				if (_animations[i].clip != pressIn)
				{
					if(events.Length < i)
					events[i].Invoke();
					anyEvent.Invoke();
					_animations[i].clip = pressIn;
					_animations[i].Play();
				}
			}
			else
			{
				if (_animations[i].clip != popOut)
				{
					_animations[i].clip = popOut;
					_animations[i].Play();
				}
			}
			
		}
	}
}
