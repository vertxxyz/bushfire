using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector2 cameraSpeed = Vector2.one;
	private Transform t;


	private void OnEnable()
	{
		t = transform;
	}

	// Update is called once per frame
	private void Update () {
		t.Rotate(Vector3.up * Input.GetAxis("Mouse X") * cameraSpeed.x, Space.World);
		t.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * cameraSpeed.y, Space.Self);
	}
}
