using UnityEngine;

public class ConstrainWorld : MonoBehaviour
{
	public Transform constrainTo;
	public Vector3 direction;

	private void Update()
	{
		transform.position = constrainTo.position + direction;
	}
}
