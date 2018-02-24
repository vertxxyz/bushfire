using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class MakeCameraLookLikeVcam : MonoBehaviour
{
	private CinemachineFreeLook _freeLook;
	private void Start()
	{
		_freeLook = GetComponent<CinemachineFreeLook>();
	}

	public void MakeCameraLookLikeVCam(CinemachineVirtualCamera vcam)
	{
		CinemachinePOV pov = vcam.GetCinemachineComponent<CinemachinePOV>();
		_freeLook.m_XAxis.Value = pov.m_HorizontalAxis.Value;
		_freeLook.m_YAxis.Value = pov.m_VerticalAxis.Value;
	}
}
