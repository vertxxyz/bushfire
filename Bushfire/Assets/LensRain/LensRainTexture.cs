using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LensRainTexture : MonoBehaviour
{
	[IntVector2]
	public Vector2 lensRainSize = new Vector2(1024, 512);

	private Camera _camera;
	private RenderTexture _renderTexture;

	public Material rainMaterial;

	private void Start()
	{
		_camera = GetComponent<Camera>();
		_renderTexture = new RenderTexture((int)lensRainSize.x, (int)lensRainSize.y, 32, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
		_camera.targetTexture = _renderTexture;
		rainMaterial.SetTexture("_RainNormal", _renderTexture);
	}

	public Material lensRainNormalMaterial;
	
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, lensRainNormalMaterial);
	}
}
