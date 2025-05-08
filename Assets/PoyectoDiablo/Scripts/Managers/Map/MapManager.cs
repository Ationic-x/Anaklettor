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

    private LoadSceneController _loadSceneController;

	public Transform enemyContainer;
    public Animator loadingAnimator;
    private RoomData _currentRoomData;
    private LevelGenerator _levelInstance;


    public LevelGenerator LevelInstance {get => _levelInstance; }

	private void Awake()
    {
        _levelInstance = LevelGenerator.Instance;
		Vector2Int pos = _levelInstance.PlayerCurrentRoom;

		_currentRoomData = LevelGenerator.Instance.GetOrCreateRoomData(pos);
		
        if (_levelInstance.MoveDirection != Vector2Int.zero) loadingAnimator.SetTrigger("Enable");
		if (_sceneDictionary.Count == 0) InitializeSceneDictionary();

		_loadSceneController = FindFirstObjectByType<LoadSceneController>();
        _loadSceneController.MapManager = this;
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

	//private void SpawnEnemiesForRoom()
 //   {

 //       if (!_currentRoomData.initialized)
 //       {

 //           for (int i = 0; i < enemyCount; i++)
 //           {
 //               var enemyData = new EnemyData();
 //               enemyData.position = new Vector3Int(UnityEngine.Random.Range(-5, 6), 0, UnityEngine.Random.Range(-5, 6));
 //               _currentRoomData.enemies.Add(enemyData);
 //           }

 //           _currentRoomData.initialized = true;
 //       }

 //       foreach (var enemy in _currentRoomData.enemies)
 //       {
 //           TestEnemy controller = enemyData.GetComponent<TestEnemy>();
 //           controller.SetData(enemy);
 //       }
 //   }

    public void ChangeScene(Vector2Int newRoom, Vector2Int direction)
    {
        _loadSceneController.ChangeScene(newRoom, direction);
    }

	//	_levelInstance.PlayerCurrentRoom = newRoom;
	//	_levelInstance.MoveDirection = direction;
	//	_levelInstance.PlayerPosition = _player.transform.position;
	//	_levelInstance.PlayerRotation = _player.transform.rotation;
}
