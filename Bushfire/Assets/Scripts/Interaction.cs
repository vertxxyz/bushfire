using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Interaction : MonoBehaviour
{
	[ReadOnly]
	public Camera _camera;

	private void Reset()
	{
		if(_camera == null)
			_camera = GetComponent<Camera>();
	}

	[ReadOnly]
	public CursorLockMode lockModeCurrent;

	public CursorLockMode lockModeIntended = CursorLockMode.Locked;
	
	
	private void Start()
	{
		if(_camera == null)
			_camera = GetComponent<Camera>();

		SetLockedModeToIntended();
	}

	private void SetLockedModeToIntended()
	{
		lockModeCurrent = lockModeIntended;
		Cursor.lockState = lockModeIntended;
	}

	private void UnlockCursor()
	{
		lockModeCurrent = CursorLockMode.None;
		Cursor.lockState = lockModeCurrent;
	}

	public HashSet<InteractorContext> currentContexts = new HashSet<InteractorContext>();

	public void AddContext(InteractorContext context)
	{
		currentContexts.Add(context);
	}
	
	public void RemoveContext(InteractorContext context)
	{
		currentContexts.Remove(context);
	}

	public bool CheckContext(InteractorContext context)
	{
		return currentContexts.Contains(context);
	}

	public LayerMask interactionLayers;
	public bool useCursor = false;

	private void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			UnlockCursor();
		}

		
		if (Input.GetMouseButtonDown(0))
		{
			if (lockModeCurrent == CursorLockMode.None)
			{
				SetLockedModeToIntended();
			}
			
			Ray r = useCursor ?
				_camera.ScreenPointToRay(Input.mousePosition) :
				new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if (Physics.Raycast(r, out hit, Mathf.Infinity, interactionLayers))
			{
				Interactor interactor = hit.collider.gameObject.GetComponent<Interactor>();
				if (interactor != null)
				{
					if (interactor.context == null)
					{
						//If an interactor doesn't have a context we can always interact with it
						interactor.MouseDown();
						return;
					}
					if (CheckContext(interactor.context))
					{
						interactor.MouseDown();
					}
				}
				else
				{
					Debug.LogWarning("Object tagged with " + interactionLayers + " doesn't have an Interactor component", hit.collider.gameObject);
					return;
				}
			}
		}
	}
}
