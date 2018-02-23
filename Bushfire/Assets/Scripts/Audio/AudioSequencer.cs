﻿using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSequencer : MonoBehaviour
{
	private AudioSource _audioSource;
	
	//There are an array of variables to ensure audio preloading.
	private const int audioSourceInstances = 2;
	private int currentPlayingInstance;
	private AudioSource[] _audioSources;
	
	private double nextEventTime;
	
	// Use this for initialization
	private void Start ()
	{
		_audioSource = GetComponent<AudioSource>();
		_audioSources = new AudioSource[audioSourceInstances];
		for (int i = 0; i < audioSourceInstances; i++)
		{
			GameObject g = new GameObject(gameObject.name + " " + i);
			g.transform.SetParent(transform);
			_audioSources[i] = CopyComponent(_audioSource, g);
		}

		_audioSources[currentPlayingInstance].clip = GetNextAudioAndIncrementSequence();
		_audioSources[currentPlayingInstance].Play();
		nextEventTime = AudioSettings.dspTime + _audioSources[currentPlayingInstance].clip.length;
	}

	private static T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}

	private void Reset()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	public FileProvider[] audioSequences;
	private int currentSequenceIndex;

	private void Update()
	{
		double time = AudioSettings.dspTime;
		if (time + 1.0F > nextEventTime) {
			IncrementCurrentPlayingInstance();
			_audioSources[currentPlayingInstance].clip = GetNextAudioAndIncrementSequence();
			_audioSources[currentPlayingInstance].PlayScheduled(nextEventTime);
			nextEventTime += _audioSources[currentPlayingInstance].clip.length;
		}
	}
	
	private void IncrementCurrentSequence()
	{
		currentSequenceIndex++;
		if (currentSequenceIndex >= audioSequences.Length)
			currentSequenceIndex = 0;
	}

	private void IncrementCurrentPlayingInstance()
	{
		currentPlayingInstance++;
		if (currentPlayingInstance >= audioSourceInstances)
			currentPlayingInstance = 0;
	}

	private AudioClip GetNextAudioAndIncrementSequence ()
	{

		AudioClip clip = audioSequences[currentSequenceIndex].GetObject<AudioClip>();
		IncrementCurrentSequence();
		return clip;
	}
}
