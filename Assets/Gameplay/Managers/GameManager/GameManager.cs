using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private const float _AnimationDelay = 0.5f;

	// Load elements
	[SerializeField] private GameObject _warpPrefab;
	
	// Curently we're hardcoding the player prefab, but in future will use a reference of the choosen one (select scene)
	[SerializeField] private GameObject _player;
	[SerializeField] private GameObject _enemy;

    [SerializeField] private GameObject _playerCamera;
	[SerializeField] private ActorData _playerData;

	[SerializeField] private SerializableDictionary<Jobs, ActorData> _listEnemies;
	[SerializeField] private SpawnRules _spawnRules;

	// Loading elements & variables
	private HUDController _hudController;
	private bool _isLoading;
	private Vector2Int _direction;

	// Map variables
	private MapManager _mapManager;

    #region Preload
    private void Awake()
	{
		_isLoading = false;
	}

	private void Start()
	{

		if (_mapManager.LevelInstance.MoveDirection != Vector2Int.zero) StartCoroutine(ExitAnimationDelayed());
	}

	public MapManager MapManager { set
		{
			_mapManager = value;		
			StartCoroutine(WaitForGameObjects());
		}
	}
	IEnumerator WaitForGameObjects()
	{
		_hudController = null;
		GameObject floor = null;
		GameObject map = null;
		GameObject spawn = null;
		GameObject warp = null;

		LevelGenerator levelInstance = _mapManager.LevelInstance;
		while (floor == null || _mapManager == null || _hudController == null || map == null || spawn == null)
		{
			floor = GameObject.FindGameObjectWithTag(Tags.Floor);
			map = GameObject.FindGameObjectWithTag(Tags.Map);
			spawn = GameObject.FindGameObjectWithTag(Tags.Spawn);
			warp = GameObject.FindGameObjectWithTag(Tags.Warp);
			_hudController = FindFirstObjectByType<HUDController>();
			yield return null;
		}

		// Warp
		WarpGenerator.Generate(levelInstance, floor, _warpPrefab, warp);

		// Rotate Map
		MapController.RotateMap(levelInstance.CurrentRoomId, floor, map);

		// Spawn
		SpawnController.Initialize(_mapManager, floor, spawn, map);
		SpawnController.SpawnPlayer(_player, _playerData, _playerCamera);
		SpawnController.SpawnEnemies(_enemy, _spawnRules, _listEnemies.ToDictionary());

		// Save
		SaveController.Initialize(levelInstance);

		// Minimap
		_hudController.Minimap.SetMinimap(levelInstance);
	}
    #endregion

    #region Load and Loading
    public void ChangeScene(Vector2Int newRoom, Vector2Int direction)
	{
		if (_isLoading) return; // Avoid enter multiple times

		_isLoading = true;
		_direction = direction;
		StartCoroutine(LoadSceneAsync(newRoom));
	}


	IEnumerator LoadSceneAsync(Vector2Int newRoom)
	{
		// Apply animation and pause
		Time.timeScale = 0f;
		
		foreach (GameObject soul in GameObject.FindGameObjectsWithTag(Tags.Soul))
		{
			soul.GetComponent<SoulController>().ForcedSearchPlayer();
		}

		while (GameObject.FindGameObjectWithTag(Tags.Soul) != null)	yield return null; // Wait for souls to be collected
		yield return new WaitForSecondsRealtime(0.1f); // Small delay to ensure if player has level up
		while (HUDController.isBusy) yield return null; // Wait for HUD to be free

		// Save values
		SaveController.SaveEnemies();
		SaveController.SavePlayer(newRoom, _direction);

		// Load animation
		_hudController.Load.LoadAnimationDirection(_mapManager.LevelInstance.MoveDirection);
		yield return new WaitForEndOfFrame();


		// Wait Async Load scene
		string sceneName = MapController.GetSceneName(_mapManager.LevelInstance.CurrentRoomId, _mapManager.LevelInstance.IsCurrentOrigin());
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;

		while (asyncLoad.progress < 0.9f)
		{
			yield return null;
		}

		// Wait animation end
		yield return new WaitForSecondsRealtime(_hudController.Load.GetClipDuration());

		// Return default values and synch all
		asyncLoad.allowSceneActivation = true;

		yield return new WaitForEndOfFrame();
	}
    #endregion

    #region End Load
    IEnumerator ExitAnimationDelayed()
	{
		// Is required enable the animation because the animator is reseted when the scene is loaded
		_hudController.Load.Enable();
		
		// Apply animation and making sure is applied
		yield return new WaitForEndOfFrame(); // synch animation
		_hudController.Load.LoadAnimationDirection(_mapManager.LevelInstance.MoveDirection);

		// Wait animation end + CD
		yield return new WaitForEndOfFrame();
		yield return new WaitForSecondsRealtime(_hudController.Load.GetClipDuration() + _AnimationDelay);

		// Return game control
		Time.timeScale = 1f;
		yield return new WaitForEndOfFrame();
	}
    #endregion
}
