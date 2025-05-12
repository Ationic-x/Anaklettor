using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
	private const float _AnimationDelay = 0.5f;

	// Load elements
	[SerializeField] private GameObject _warpPrefab;
	
	// Curently we're hardcoding the player prefab, but in future will use a reference of the choosen one (select scene)
	[SerializeField] private GameObject _player;
	[SerializeField] private SerializableDictionary<Rol, ActorDataSO> _listEnemies;
	[SerializeField] private SpawnRules _spawnRules;

	// Loading elements & variables
	private Animator _loadingAnimator;
	private bool _isLoading;
	private Vector2Int _direction;

	// Map variables
	private MapManager _mapManager;
	private GameObject _floor;
	private GameObject _spawn;

    #region Preload
    private void Awake()
	{
		_isLoading = false;
		_loadingAnimator = GetComponentInChildren<Animator>();

        GetComponentInChildren<TextMeshProUGUI>().text = "Cargando..."; // In case of multiple languages
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
		MinimapRenderer minimap = null; // Minimap isnt reused, so we can use a local variable
		LevelGenerator levelInstance = _mapManager.LevelInstance;
		while (_floor == null || _mapManager == null || minimap == null)
		{
			_floor = GameObject.FindGameObjectWithTag(Tags.Floor);
			_spawn = GameObject.FindGameObjectWithTag(Tags.Spawn);
			minimap = FindFirstObjectByType<MinimapRenderer>();
			yield return null;
		}

		// Warp
		WarpGenerator.Generate(levelInstance, _floor, _warpPrefab);

		// Spawn
		SpawnController.Init(_mapManager, _floor, _spawn);
		SpawnController.SpawnPlayer(_player);
		SpawnController.SpawnEnemies(_spawnRules, _listEnemies.ToDictionary());

		// Save
		SaveController.Initialize(levelInstance);

		// Minimap
		minimap.SetMinimap(levelInstance);
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

	private void LoadAnimationDirection()
	{
		Vector2Int direction = _mapManager.LevelInstance.MoveDirection;

		string animationDirection = direction == Vector2Int.up ? "Top" :
					  direction == Vector2Int.down ? "Bottom" :
					  direction == Vector2Int.left ? "Left" :
					  "Right";

		_loadingAnimator.SetTrigger(animationDirection);
	}


	IEnumerator LoadSceneAsync(Vector2Int newRoom)
	{
		// Apply animation and pause
		Time.timeScale = 0f;

		// Save values
		SaveController.SaveEnemies(_listEnemies[0]);
		SaveController.SavePlayer(newRoom, _direction);

		yield return new WaitForEndOfFrame(); // synch animation
		LoadAnimationDirection();
		yield return new WaitForEndOfFrame();


		// Wait Async Load end
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		asyncLoad.allowSceneActivation = false;

		while (asyncLoad.progress < 0.9f)
		{
			yield return null;
		}

		// Wait animation end
		float waitTime = _loadingAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

		yield return new WaitForSecondsRealtime(waitTime);

		// Return default values and synch all
		asyncLoad.allowSceneActivation = true;

		yield return new WaitForEndOfFrame();
	}
    #endregion

    #region End Load
    IEnumerator ExitAnimationDelayed()
	{
		// Is required enable the animation because the animator is reseted when the scene is loaded
		_loadingAnimator.SetTrigger("Enable");
		// Apply animation and making sure is applied
		yield return new WaitForEndOfFrame(); // synch animation
		LoadAnimationDirection();
		yield return new WaitForEndOfFrame();

		// Wait animation end + CD
		float animDuration = _loadingAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		yield return new WaitForSecondsRealtime(animDuration + _AnimationDelay);

		// Return game control
		Time.timeScale = 1f;
		yield return new WaitForEndOfFrame();
	}
    #endregion
}
