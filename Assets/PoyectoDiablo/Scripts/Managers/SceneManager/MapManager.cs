using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class MapManager : MonoBehaviour
{
    public TextMeshProUGUI positionText;
    private static Dictionary<int, Tuple<int, float>> _sceneDictionary = new Dictionary<int, Tuple<int, float>>();
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public Transform enemyContainer;
    public Animator loadingAnimator;
    private bool _isLoading = false;
    private RoomData _currentRoomData;
    private LevelGenerator _levelInstance;
    private GameObject _player;

	private void Awake()
    {
        _levelInstance = LevelGenerator.Instance;
		Vector2Int pos = _levelInstance.PlayerCurrentRoom;

		_currentRoomData = LevelGenerator.Instance.GetOrCreateRoomData(pos);
		
        SpawnEnemiesForRoom();
        ModifyPlayerTransform();
        if (_levelInstance.MoveDirection != Vector2Int.zero) loadingAnimator.SetTrigger("Enable");
		if (_sceneDictionary.Count == 0) InitializeSceneDictionary();

		positionText.text = $"Sala actual: {pos.x}, {pos.y}, ID: {_levelInstance.GetMapValue(pos) } +  Rotation: {_sceneDictionary[_levelInstance.GetMapValue(pos)]}";
	}

	private void Start()
	{
		if (_levelInstance.MoveDirection != Vector2Int.zero) StartCoroutine(ExitAnimationDelayed(0.01f));
	}

	private void InitializeSceneDictionary()
	{
		// Map Value, Scene(ID, Rotation)
		_sceneDictionary = new Dictionary<int, Tuple<int, float>>
		{
            // Start And Endpoints
            { 1, new Tuple<int, float>(0, 0f)},   // O -> North
            { 9, new Tuple<int, float>(0, 180f)},   // O -> South
            { 17, new Tuple<int, float>(0, 90f)},  // O -> East
            { 25, new Tuple<int, float>(0, 270f)},  // O -> West
            
            // ID 1 is reserversd for the end point

            // Path
            { 2, new Tuple<int, float>(2, 0f)},   // North -> South
            { 4, new Tuple<int, float>(3, 0f)}, // North -> East
            { 6, new Tuple<int, float>(3, -0f)}, // North -> West

            { 8, new Tuple<int, float>(2, 180f)} , // South -> North
            { 12, new Tuple<int, float>(3, 180f)}, // South -> East
            { 14, new Tuple<int, float>(3, -180f)}, // South -> West
                        
            { 16, new Tuple<int, float>(2, -90f)} , // East -> North
            { 18, new Tuple<int, float>(3, 90f)}, // East -> South
            { 22, new Tuple<int, float>(3, 90f)}, // East -> West
                        
            { 24, new Tuple<int, float>(2, -270f)}, // West -> North
            { 26, new Tuple<int, float>(3, 270f)}, // West -> South
            { 28, new Tuple<int, float>(3, 270f)}, // West -> East

            // Random non-path
            { 32, new Tuple<int, float>(4, 0f)},
		};
	}

	private void SpawnEnemiesForRoom()
    {

        if (!_currentRoomData.initialized)
        {
            int enemyCount = UnityEngine.Random.Range(3, 6);

            for (int i = 0; i < enemyCount; i++)
            {
                var enemyData = new EnemyData();
                enemyData.position = new Vector3Int(UnityEngine.Random.Range(-5, 6), 0, UnityEngine.Random.Range(-5, 6));
                _currentRoomData.enemies.Add(enemyData);
            }

            _currentRoomData.initialized = true;
        }

        foreach (var enemy in _currentRoomData.enemies)
        {
            if (!enemy.isAlive)
                continue;

            var enemyData = Instantiate(enemyPrefab, enemyContainer);
            enemyData.transform.position = new Vector3(enemy.position.x, enemy.position.y, enemy.position.z);

            TestEnemy controller = enemyData.GetComponent<TestEnemy>();
            controller.SetData(enemy);
        }
    }

    private void ModifyPlayerTransform()
    {
		Vector2Int dir = _levelInstance.MoveDirection;

		Vector3 position = dir == Vector2Int.zero? Vector3.zero : _levelInstance.PlayerPosition;
        Quaternion rotation = dir == Vector2Int.zero ? new Quaternion(0f, 180f, 0f, 0f) : _levelInstance.PlayerRotation;
		Vector3 flip = Vector3.one;

		if (dir == Vector2Int.up || dir == Vector2Int.down)
			flip.z = -0.9f;
		else if (dir == Vector2Int.left || dir == Vector2Int.right)
			flip.x = -0.9f;

		_player = Instantiate(playerPrefab, Vector3.Scale(position, flip), rotation);
		_player.GetComponentInChildren<TestMovement>().SetMapManager(this);
	}

    public void ChangeScene(Vector2Int direction)
    {
        if (_isLoading) return;

        if (direction != Vector2Int.zero)
        {
            Vector2Int newPos = LevelGenerator.Instance.PlayerCurrentRoom + direction;
            if (LevelGenerator.Instance.IsValid(newPos))
            {
                _isLoading = true;
                StartCoroutine(LoadSceneAsync(newPos, direction));
            }
        }
    }

    IEnumerator ExitAnimationDelayed(float delay)
	{
		// Animations work in frame reason why we work with WaitForEndOfFrame
		yield return new WaitForEndOfFrame(); // wait enable animation is applied
		TriggerLoadAnimation(_levelInstance.MoveDirection);
		yield return new WaitForEndOfFrame(); // wait move direction animation is applied

		float waitTime = loadingAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		yield return new WaitForSecondsRealtime(waitTime + 1f); // Wait the end of animation before allow player movement
		Time.timeScale = 1f;
		yield return new WaitForEndOfFrame();
	}

	IEnumerator LoadSceneAsync(Vector2Int newPos, Vector2Int direction)
	{
        TriggerLoadAnimation(direction);
		positionText.text = "Cargando...";
		yield return new WaitForEndOfFrame(); // wait move direction animation is applied

		_levelInstance.PlayerCurrentRoom = newPos;
		_levelInstance.MoveDirection = direction;
		_levelInstance.PlayerPosition = _player.transform.position;
		_levelInstance.PlayerRotation = _player.transform.rotation;

		Time.timeScale = 0f;


		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
		asyncLoad.allowSceneActivation = false;

		while (asyncLoad.progress < 0.9f)
		{
			yield return null;
		}

		
        float waitTime = loadingAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

		yield return new WaitForSecondsRealtime(waitTime);

		asyncLoad.allowSceneActivation = true;

		yield return new WaitForEndOfFrame(); // synch animations with scene load
	}

    private void TriggerLoadAnimation(Vector2Int direction)
    {
		string animationDirection = direction == Vector2Int.up ? "Top" :
					  direction == Vector2Int.down ? "Bottom" :
					  direction == Vector2Int.left ? "Left" :
					  "Right";

		loadingAnimator.SetTrigger(animationDirection);
	}
}
