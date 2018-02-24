using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FileProvider)), RequireComponent(typeof(AudioSource))]
public class PlayAudioOnce : MonoBehaviour {

	private AudioSource _audioSource;
	private FileProvider _fileProvider;

	private void Reset()
	{
		if(_audioSource == null)
			_audioSource = GetComponent<AudioSource>();
		if (_fileProvider == null)
			_fileProvider = GetComponent<FileProvider>();
	}

	private void Start()
	{
		Reset();
	}
	
	public void Play()
	{
		_audioSource.PlayOneShot(_fileProvider.GetObject<AudioClip>());
	}
}
