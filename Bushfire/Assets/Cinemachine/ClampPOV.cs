using UnityEngine;
using Cinemachine;
 
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class ClampPOV : MonoBehaviour
{
	CinemachineVirtualCamera _virtualCamera;
	[Range(0, 360)]
	public float deadZoneSizeX = 60;
	
	[Range(0, 360)]
	public float deadZoneSizeY = 60;

	private CinemachinePOV pov;
	
	// Use this for initialization
	void Start()
	{
		pov = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();
	}
   
	// Update is called once per frame
	public void Update()
	{
		float axisLookX = pov.m_HorizontalAxis.Value;
		float halfDeadZoneLookX = deadZoneSizeX / 2;
		if (axisLookX > halfDeadZoneLookX)
			axisLookX = Mathf.Min(axisLookX, halfDeadZoneLookX);
		if (axisLookX < -halfDeadZoneLookX)
			axisLookX = Mathf.Max(axisLookX, -halfDeadZoneLookX);
		pov.m_HorizontalAxis.Value = axisLookX;
		
		float axisLookY = pov.m_VerticalAxis.Value;
		float halfDeadZoneLookY = deadZoneSizeY / 2;
		if (axisLookY > halfDeadZoneLookY)
			axisLookY = Mathf.Min(axisLookY, halfDeadZoneLookY);
		if (axisLookY < -halfDeadZoneLookY)
			axisLookY = Mathf.Max(axisLookY, -halfDeadZoneLookY);
		pov.m_VerticalAxis.Value = axisLookY;
	}
}