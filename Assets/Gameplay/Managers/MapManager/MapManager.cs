using UnityEngine;

public class MapManager : MonoBehaviour
{

	private GameManager _loadSceneController;

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

		_loadSceneController = FindFirstObjectByType<GameManager>();
		_loadSceneController.MapManager = this;
	}

	public void ChangeScene(Vector2Int newRoom, Vector2Int direction)
    {
        _loadSceneController.ChangeScene(newRoom, direction);
    }
}
