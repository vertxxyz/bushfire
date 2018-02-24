using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class MakeCameraLike : MonoBehaviour
{
	private CinemachineVirtualCamera vCam;
	private void Start()
	{
		vCam = GetComponent<CinemachineVirtualCamera>();
	}

	public void MakeCameraSameAs(Transform transform_)
	{
		vCam.transform.rotation = transform_.rotation;
	}
}
