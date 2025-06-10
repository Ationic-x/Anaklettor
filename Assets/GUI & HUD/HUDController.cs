using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private MinimapRenderer _minimapRenderer;
    [SerializeField] private LoadGUIController _loadGUIController;
    [SerializeField] private LevelGUIController _levelGUIController;
    [SerializeField] private VictoryGUIController _victoryGUIController;
    [SerializeField] private GameOverGUIController _gameOverGUIController;

    public static bool isBusy;

    private void Start()
    {
        EnableAllChildren();
	}

    private void OnEnable()
    {
        _levelGUIController.OnLevelUpAction += IsBusy;
        _victoryGUIController.OnEnable += IsBusy;
        _gameOverGUIController.OnEnable += IsBusy;
    }

    private void OnDisable()
    {
        _levelGUIController.OnLevelUpAction -= IsBusy;
        _victoryGUIController.OnEnable -= IsBusy;
        _gameOverGUIController.OnEnable -= IsBusy;
    }

    private void Awake()
    {
        isBusy = false;
    }

    private void IsBusy(bool value)
    {
        isBusy = value;
    }

    private void EnableAllChildren()
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}
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

    public VictoryGUIController Victory
    {
        get => _victoryGUIController;
    }

    public GameOverGUIController GameOver
    {
        get => _gameOverGUIController;
    }


}
