using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector2 cameraSpeed = Vector2.one;
	public bool useAsCamera;
	private Transform t;

	private bool isLocked;

	private void OnEnable()
	{
		t = transform;
		Cursor.lockState = CursorLockMode.Locked;
		isLocked = true;
	}

	// Update is called once per frame
	private void Update () {
		if (useAsCamera)
		{
			t.Rotate(Vector3.up * Input.GetAxis("Mouse X") * cameraSpeed.x, Space.World);
			t.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * cameraSpeed.y, Space.Self);
		}

		if (Input.GetKey(KeyCode.Escape))
		{
			isLocked = false;
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (!isLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				isLocked = true;
			}
		}
			
	}
}
