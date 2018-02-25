using UnityEngine;

public class SteeringWheel : MonoBehaviour
{

	private Quaternion startRotation;

	public float angleMultiplier = 1;
	public float interpSpeed = 10;
	
	// Use this for initialization
	private void Start ()
	{
		startRotation = transform.localRotation;
	}

	public void UpdateAngle(float angle)
	{
		transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation * Quaternion.AngleAxis(angle * angleMultiplier, Vector3.up), interpSpeed * Time.deltaTime);
	}
}
