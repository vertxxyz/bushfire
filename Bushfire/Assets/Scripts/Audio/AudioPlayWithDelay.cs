using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayWithDelay : MonoBehaviour
{
	private AudioSource _audioSource;
	public AudioClip[] audioClips;

	private void Reset()
	{
		if(_audioSource == null)
			_audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		Reset();
	}

	public void PlayAudioClipAtIndex(int index)
	{
		if (index >= audioClips.Length)
		{
			Debug.LogWarning("Provided index is not valid " + index, gameObject);
			return;
		}
		_audioSource.clip = audioClips[index];
		StartCoroutine(PlayWithDelay());
	}

	public float delay = 0.1f;

	IEnumerator PlayWithDelay()
	{
		yield return new WaitForSeconds(delay);
		_audioSource.Play();
	}
}
