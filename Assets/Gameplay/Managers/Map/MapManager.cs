using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


[Serializable]
public struct RoomSceneData
{
	public int sceneId;
	public float rotation;

	public RoomSceneData(int id, float rotation)
	{
		sceneId = id;
		this.rotation = rotation;
	}
}

public class MapManager : MonoBehaviour
{
	private static Dictionary<int, RoomSceneData> _sceneDictionary = new();

	private LoadSceneController _loadSceneController;

    private RoomData _currentRoomData;
    private LevelGenerator _levelInstance;


    public LevelGenerator LevelInstance {get => _levelInstance; }

	private void Awake()
	{
		if (LevelGenerator.Instance != null)
		{
			Initialize();
		}
		else
		{
			LevelGenerator.OnLevelGeneratorReady += Initialize;
		}
	}

	private void Initialize()
	{
		_levelInstance = LevelGenerator.Instance;
		
		_levelInstance.TryGetCurrentRoomData(out RoomData roomData);
		_currentRoomData = roomData;

		if (_sceneDictionary.Count == 0)
			InitializeSceneDictionary();

		_loadSceneController = FindFirstObjectByType<LoadSceneController>();
		_loadSceneController.MapManager = this;
	}

	private void InitializeSceneDictionary()
	{
		// Map Value, Scene(ID, Rotation)
		_sceneDictionary = new Dictionary<int, RoomSceneData>
		{
			// Start And Endpoints
			{ 1, new RoomSceneData(0, 0f) },     // O -> North
			{ 9, new RoomSceneData(0, 180f) },   // O -> South
			{ 17, new RoomSceneData(0, 90f) },   // O -> East
			{ 25, new RoomSceneData(0, 270f) },  // O -> West

			// ID 1 is reserved for the end point

			// Path
			{ 2, new RoomSceneData(2, 0f) },     // North -> South
			{ 4, new RoomSceneData(3, 0f) },     // North -> East
			{ 6, new RoomSceneData(3, -0f) },    // North -> West

			{ 8, new RoomSceneData(2, 180f) },   // South -> North
			{ 12, new RoomSceneData(3, 180f) },  // South -> East
			{ 14, new RoomSceneData(3, -180f) }, // South -> West

			{ 16, new RoomSceneData(2, -90f) },  // East -> North
			{ 18, new RoomSceneData(3, 90f) },   // East -> South
			{ 22, new RoomSceneData(3, 90f) },   // East -> West

			{ 24, new RoomSceneData(2, -270f) }, // West -> North
			{ 26, new RoomSceneData(3, 270f) },  // West -> South
			{ 28, new RoomSceneData(3, 270f) },  // West -> East

			// Random non-path
			{ 32, new RoomSceneData(4, 0f) }
		};
	}

	public void ChangeScene(Vector2Int newRoom, Vector2Int direction)
    {
        _loadSceneController.ChangeScene(newRoom, direction);
    }
}
