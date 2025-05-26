using UnityEngine;

public class HUDController : MonoBehaviour
{
	[SerializeField] private MinimapRenderer _minimapRenderer;
	[SerializeField] private LoadAnimatorController _loadAnimatorController;
	
	public MinimapRenderer Minimap
	{
		get => _minimapRenderer;
	}

	public LoadAnimatorController Load
	{
		get => _loadAnimatorController;
	}
}
