using System;
using UnityEngine;

public class HUDController : MonoBehaviour
{
	[SerializeField] private MinimapRenderer _minimapRenderer;
	[SerializeField] private LoadGUIController _loadGUIController;
	[SerializeField] private LevelGUIController _levelGUIController;

	public static bool isBusy;

	private void OnEnable()
	{
		_levelGUIController.OnLevelUpAction += IsBusy;
	}

	private void OnDisable()
	{
		_levelGUIController.OnLevelUpAction -= IsBusy;
	}

	private void IsBusy(bool value)
	{
		isBusy = value;
	}
	
	public MinimapRenderer Minimap
	{
		get => _minimapRenderer;
	}

	public LoadGUIController Load
	{
		get => _loadGUIController;
	}
	
	public LevelGUIController Level
	{
		get => _levelGUIController;
	}
}
