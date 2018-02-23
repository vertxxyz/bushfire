using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioAtRandomInterval : MonoBehaviour
{
	[MinMax("Interval", 0, 100)]
	public float intervalMin = 2;
	[HideInInspector]
	public float intervalMax = 10;

	public FileRandomiser fileRandomiser;
	[SerializeField, ReadOnly]
	private AudioSource _audioSource;
	
	private void Reset()
	{
		fileRandomiser = GetComponent<FileRandomiser>();
		_audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		StartCoroutine(PlayAudioAfterWait());
	}


	IEnumerator PlayAudioAfterWait()
	{
		yield return new WaitForSeconds(Random.Range(intervalMin, intervalMax));
		AudioClip audioClip = fileRandomiser.GetRandomObject<AudioClip>();
		_audioSource.PlayOneShot(audioClip);
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}
}
