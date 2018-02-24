using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour {
	public UnityEvent onMouseDown;
	[Header("If an interactor doesn't have a context we can always interact with it")]
	public InteractorContext context;

	[EditorButton]
	void FindContext()
	{
		if (context == null)
		{
			Transform parent = transform;
			while (parent != null)
			{
				context = parent.GetComponent<InteractorContext>();
				if (context != null)
					break;
				parent = parent.parent;
			}
		}
	}

	public void MouseDown()
	{
		onMouseDown.Invoke();
	}
}
