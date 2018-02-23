using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(AudioSource))]
public class AudioSequencer : MonoBehaviour
{
	private PlayableDirector _playableDirector;
	// Use this for initialization
	void Start ()
	{
		_playableDirector = gameObject.AddComponent<PlayableDirector>();
		_timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
		_audioTrack = _timelineAsset.CreateTrack<AudioTrack>(null, "Audio Track");
		_playableDirector.playableAsset = _timelineAsset;
		CreateSequence();
	}

	private void Reset()
	{
		_audioSource = GetComponent<AudioSource>();
	}

	private TimelineAsset _timelineAsset;
	private AudioSource _audioSource;
	private AudioTrack _audioTrack;

	public FileRandomiser[] audioSequences;

	void CreateSequence()
	{
		_timelineAsset.DeleteTrack(_audioTrack);
		_audioTrack = _timelineAsset.CreateTrack<AudioTrack>(null, "Audio Track");
		_playableDirector.SetGenericBinding(_audioTrack, _audioSource);
		
		double start = 0;
		foreach (var audioSequence in audioSequences)
		{
			AudioClip audioClip = audioSequence.GetRandomObject<AudioClip>();
			TimelineClip clip = _audioTrack.CreateClip(audioClip);
			clip.start = start;
			start = clip.end;
		}
	}
}
