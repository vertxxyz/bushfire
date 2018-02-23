using UnityEngine;
using Cinemachine;
 
public class ClampFreeLookX : MonoBehaviour
{
	CinemachineFreeLook mFreeLook;
	[Range(0, 360)]
	public float deadZoneSize = 60;
	
	[Range(0, 360)]
	public float deadZoneSizeLook = 60;

	private CinemachinePOV[] povs;
	
	// Use this for initialization
	void Start()
	{
		mFreeLook = GetComponent<CinemachineFreeLook>();
		povs = new CinemachinePOV[3];
		for (int i = 0; i < 3; i++)
		{
			CinemachineVirtualCamera rig = mFreeLook.GetRig(i);
			povs[i] = rig.GetCinemachineComponent<CinemachinePOV>();
		}
	}
   
	// Update is called once per frame
	void Update()
	{
		float halfDeadZone = deadZoneSize / 2;
		float axis = mFreeLook.m_XAxis.Value;
		if (axis > 180)
			axis = Mathf.Max(axis, 180 + halfDeadZone);
		if (axis < 180)
			axis = Mathf.Min(axis, 180 - halfDeadZone);
		mFreeLook.m_XAxis.Value = axis;

		for (int i = 0; i < 3; i++)
		{
			CinemachinePOV pov = povs[i];
			float axisLook = Mathf.Repeat(pov.m_HorizontalAxis.Value, 360);
			float halfDeadZoneLook = deadZoneSizeLook / 2;
			if (axisLook > 180)
				axisLook = Mathf.Max(axisLook, 180 + halfDeadZoneLook);
			if (axisLook < 180)
				axisLook = Mathf.Min(axisLook, 180 - halfDeadZoneLook);
			pov.m_HorizontalAxis.Value = axisLook;
		}
	}
}