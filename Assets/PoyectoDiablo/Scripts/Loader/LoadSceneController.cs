using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneController : MonoBehaviour
{
	// Load elements
	[SerializeField] private GameObject _warpPrefab;
	// Curently we're hardcoding the player prefab, but in future will use a reference of the choosen one (select scene)
	[SerializeField] private GameObject _player;
	[SerializeField] private SerializableDictionary<Rol, GameObject> _listEnemies;
	[SerializeField] private SpawnRules _spawnRules;

	// Loading elements & variables
	private Animator _loadingAnimator;
	private bool _isLoading;
	const float _AnimationDelay = 0.5f;

	// Map variables
	private MapManager _mapManager;
	private GameObject _floor;

	private void Awake()
	{
		_isLoading = false;
		_loadingAnimator = GetComponentInChildren<Animator>();
		GetComponentInChildren<TextMeshProUGUI>().text = "Cargando..."; // In case of multiple languages
	}

	private void Start()
	{
		if (_mapManager.LevelInstance.MoveDirection != Vector2Int.zero) StartCoroutine(ExitAnimationDelayed(0.01f));	
	}

	IEnumerator WaitForFloorAndMap()
	{
		while (_floor == null || _mapManager == null)
		{
			_floor = GameObject.FindGameObjectWithTag(Tags.Floor);
			yield return null;
		}

		WarpGenerator.Generate(_mapManager, _floor, _warpPrefab);
		SpawnController.SpawnPlayer(_mapManager, _floor, _player);
		SpawnController.SpawnEnemies(_spawnRules, _floor, _listEnemies.ToDictionary());
	}

	public MapManager MapManager { set
		{
			_mapManager = value;
			StartCoroutine(WaitForFloorAndMap());
		}
	}

	public void ChangeScene(Vector2Int newRoom, Vector2Int direction)
	{
		if (_isLoading) return;
		_isLoading = true;

		_mapManager.LevelInstance.MoveDirection = direction;
		StartCoroutine(LoadSceneAsync(newRoom, direction));
	}

	private void LoadAnimationDirection(Vector2Int direction)
	{
		string animationDirection = direction == Vector2Int.up ? "Top" :
					  direction == Vector2Int.down ? "Bottom" :
					  direction == Vector2Int.left ? "Left" :
					  "Right";

		_loadingAnimator.SetTrigger(animationDirection);
	}

	IEnumerator ExitAnimationDelayed(float delay)
	{
		// Apply animation and making sure is applied
		yield return new WaitForEndOfFrame();
		LoadAnimationDirection(_mapManager.LevelInstance.MoveDirection);
		yield return new WaitForEndOfFrame();

		// Wait animation end + CD
		float animDuration = _loadingAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		yield return new WaitForSecondsRealtime(animDuration + _AnimationDelay);

		// Return game control
		Time.timeScale = 1f;
		yield return new WaitForEndOfFrame();
	}

	IEnumerator LoadSceneAsync(Vector2Int newRoom, Vector2Int direction)
	{
		// Apply animation and pause
		Time.timeScale = 0f;

		LoadAnimationDirection(direction);
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
}
