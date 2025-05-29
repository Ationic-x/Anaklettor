using System;
using System.Collections.Generic;
using UnityEngine;


// Path Sketch
// ----[00]-----
// |           |
// |           |
//[11]        [10]
// |           |
// |           |
// ----[01]-----
// Horizontal 1X
// Vertical 0X
// NorthEast X0
// SouthWest X1
//
// Start point 0_00_XX_1
// End point 0_XX_00_1
public class LevelGenerator : MonoBehaviour
{
	// This const is linked to the shader
	private const int MaxVisited = 64;

	private int option = 0b0_00_00_0; // Option: [0] non-path [00] to [00] from [0] origin/end
    public static LevelGenerator Instance { get; private set; }
	
    public static Action OnLevelGeneratorReady;

	private int _mapWidth = 6;
    private int _mapHeight = 6;
    private int _maxDistance = 4;

    private Vector2Int _origin = new Vector2Int(3, 3);

	// Player Information
	private Vector2Int _playerCurrentRoom;

    // GameData
    private Dictionary<Vector2Int, RoomData> _roomMap = new();
    private PlayerData _playerData = new PlayerData();

    private int[,] _mapData;
    private ArrayQueue<Vector4> _visitedRooms = new(MaxVisited, new Vector4(-1, -1, -1, -1));

    // Reproduce the shader animations
    // This should be in other object, but currently we're using this one
    // Cuz is a singleton with dont destroy
	private void Update()
	{
		Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
		Shader.SetGlobalFloat("_UnscaledDeltaTime", Time.unscaledDeltaTime);
	}

	private void Awake()
    {
        if (Instance != null) return;

        Instance = this;
        DontDestroyOnLoad(gameObject);
		_mapData = GenerateMap(_mapWidth, _mapHeight, _origin, _maxDistance);
		PrintMap(_mapData);
		OnLevelGeneratorReady?.Invoke();
	}

	public void DestroyInstance()
	{
		DestroyImmediate(gameObject);
	}

	private int[,] GenerateMap(int width, int height, Vector2Int origin, int steps)
    {
        _playerCurrentRoom = origin;

        int[,] map = new int[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = UnityEngine.Random.Range(32, 36); // 32 or higher

        List<Vector2Int> possibleDestinations = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Mathf.Abs(x - origin.x) + Mathf.Abs(y - origin.y) == steps)
                {
                    possibleDestinations.Add(new Vector2Int(x, y));
                }
            }
        }

        if (possibleDestinations.Count == 0)
        {
            Debug.LogError("There are possible paths");
            return map;
        }

        Vector2Int target = possibleDestinations[UnityEngine.Random.Range(0, possibleDestinations.Count)];

        List<Vector2Int> path = GeneratePath(origin, target);



        for (int i = 0; i < path.Count - 1; i++)
        {
            ClearOption(); // Make sure is cleared
            Vector2Int current = path[i];
            Vector2Int next = path[i + 1];

            // Origin
            if (i == 0) option |= 0b0_00_00_1; // Origin
            else SetOptionOrigin(map[path[i - 1].x, path[i - 1].y]);

            // Destination
            if (current.x < next.x) option |= 0b0_10_00_0; // East
            else if (current.x > next.x) option |= 0b0_11_00_0; // West
            else if (current.y > next.y) option |= 0b0_01_00_0; // South
            // North 0b0_00_00_0

            map[current.x, current.y] = option; // Set the value of the current position
        }
        ClearOption();
        SetOptionOrigin(map[path[^2].x, path[^2].y]);
        option += 0b1;

        map[target.x, target.y] = option;

        return map;
    }

    private void ClearOption()
    {
        option = 0b0_00_00_0;
    }

    private void SetOptionOrigin(int previousValue)
    {
        option |= ((previousValue >> 3) ^ 0b1) << 1;
    }


    #region Path Generation
    private List<Vector2Int> GeneratePath(Vector2Int origin, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int> { origin };
        Vector2Int current = origin;

        while (current != target)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                if (target.x != current.x) path.Add(StepX(current, target));
                else if (target.y != current.y) path.Add(StepY(current, target));
            }
            else
            {
                if (target.y != current.y) path.Add(StepY(current, target));
                else if (target.x != current.x) path.Add(StepX(current, target));
            }

            current = path[^1];
        }

        return path;
    }

    private Vector2Int StepX(Vector2Int current, Vector2Int target)
    {
        if (target.x > current.x) return new Vector2Int(current.x + 1, current.y);
        else if (target.x < current.x) return new Vector2Int(current.x - 1, current.y);
        return current;
    }

    private Vector2Int StepY(Vector2Int current, Vector2Int target)
    {
        if (target.y > current.y) return new Vector2Int(current.x, current.y + 1);
        else if (target.y < current.y) return new Vector2Int(current.x, current.y - 1);
        return current;
    }
    #endregion

    #region Map Checks
    public int GetMapValue(Vector2Int pos)
	{
        return _mapData[pos.x, pos.y];
	}

    public bool IsOrigin(Vector2Int pos)
    {
		return (pos == _origin);
    }

    public bool IsCurrentOrigin()
    {
        return IsOrigin(PlayerCurrentRoom);
    }

	public bool IsValid(Vector2Int pos)
    {
		return pos.x >= 0 && pos.x < _mapWidth && pos.y >= 0 && pos.y < _mapHeight;
    }
	#endregion

	#region Getters and Setters
	public PlayerData PlayerData { get { return _playerData; } }
    
    public bool TryGetRoomData(Vector2Int position, out RoomData roomData)
	{
		return _roomMap.TryGetValue(position, out roomData);
	}

	public bool TryGetCurrentRoomData(out RoomData roomData)
	{
		return TryGetRoomData(_playerCurrentRoom, out roomData);
	}

	public RoomData NewRoomData(Vector2Int position)
	{
		_roomMap[position] = new RoomData();

		return _roomMap[position];
	}

	public RoomData NewCurrentRoomData()
	{
		return NewRoomData(_playerCurrentRoom);
	}

    public int CurrentRoomId
    { 
        get => _mapData[_playerCurrentRoom.x, _playerCurrentRoom.y]; 
    }

	public Vector2Int PlayerCurrentRoom
    {
        get { return _playerCurrentRoom; }
        set 
        { 
            _visitedRooms.PushHash(new Vector4(_playerCurrentRoom.x, _playerCurrentRoom.y, 0, 0)); // Store previous value without repeating
			_playerCurrentRoom = value; 
        }
    }

	public int MapWidth
	{
		get { return _mapWidth; }
		set { _mapWidth = value; }
	}

    public ArrayQueue<Vector4> VisitedRooms
    {
        get { return _visitedRooms; }
	}

	public int VisitedCount
	{
		get { return _visitedRooms.Count; }
	}
	#endregion

	#region Debug
	public void PrintMap(int[,] mapData)
	{
		string output = "";
		for (int y = mapData.GetLength(1) - 1; y >= 0; y--)
		{
			for (int x = 0; x < mapData.GetLength(0); x++)
			{
				output += mapData[x, y] + " ";
			}
			output += "\n";
		}
		Debug.Log(output);
	}
	#endregion
}
