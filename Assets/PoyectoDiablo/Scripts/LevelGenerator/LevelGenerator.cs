using System.Collections.Generic;
using UnityEngine;


// Level numbers
// 0 = origin
// 1 = vertical
// 2 = horizontal
// 3 = up-right 
// 4 = up-left
// 5 = down-right
// 6 = down-left
// 7 = left-up
// 8 = left-down
// 9 = right-up
// 10 = right-down
// 11+ = random maps
public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator Instance { get; private set; } = null;

    private int _mapWidth = 10;
    private int _mapHeight = 10;
    private int _maxDistance = 5;

    private Vector2Int _origin = new Vector2Int(3, 3);
    private Vector2Int _playerCurrentRoom;

    private Dictionary<Vector2Int, RoomData> _roomMap = new();


    // Just for debugging
    private int[,] _mapData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _mapData = GenerateMap(_mapWidth, _mapHeight, _origin, _maxDistance);
            PrintMap(_mapData);
        }
    }

    private int[,] GenerateMap(int width, int height, Vector2Int origin, int steps)
    {
        int[,] map = new int[width, height];
        _playerCurrentRoom = origin;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = 15;

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
            Debug.LogError("No hay destinos posibles para esa distancia");
            return map;
        }

        Vector2Int target = possibleDestinations[Random.Range(0, possibleDestinations.Count)];

        List<Vector2Int> path = GeneratePath(origin, target);

        map[origin.x, origin.y] = 0;
        int lastValue = 0;
        int lastY = origin.y;
        int lastX = origin.x;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2Int pos = path[i];

            if ((lastY - pos.y) != 0)
            {
                if(lastValue == 2)
                {
                    if (origin.x - pos.x < 0)
                    {
                        if (origin.y - pos.y < 0)
                        {
                            map[lastX, lastY] = 7;
                        } else
                        {
                            map[lastX, lastY] = 8;
                        }
                    } else
                    {
                        if (origin.y - pos.y < 0)
                        {
                            map[lastX, lastY] = 9;
                        }
                        else
                        {
                            map[lastX, lastY] = 10;
                        }
                    }
                }
                lastValue = 1;
                map[pos.x, pos.y] = 1; // Vertical

            } else
            {
                if (lastValue == 1)
                {
                    if (origin.y - pos.y < 0)
                    {
                        if (origin.x - pos.x < 0)
                        {
                            map[lastX, lastY] = 3;
                        }
                        else
                        {
                            map[lastX, lastY] = 4;
                        }
                    }
                    else
                    {
                        if (origin.x - pos.x < 0)
                        {
                            map[lastX, lastY] = 5;
                        }
                        else
                        {
                            map[lastX, lastY] = 6;
                        }
                    }
                }
                lastValue = 2;
                map[pos.x, pos.y] = 2; // Horizontal
            }

            lastY = pos.y;
            lastX = pos.x;
        }

        return map;
    }

    private List<Vector2Int> GeneratePath(Vector2Int origin, Vector2Int target)
    {
        List<Vector2Int> path = new List<Vector2Int> { origin };
        Vector2Int current = origin;

        while (current != target)
        {
            if (Random.Range(0, 2) == 0)
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

    public bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < _mapWidth && pos.y >= 0 && pos.y < _mapHeight
            && _mapData[pos.x, pos.y] != 2;
    }

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

    // Getters and Setters
    public RoomData GetOrCreateRoomData(Vector2Int position)
    {
        if (!_roomMap.ContainsKey(position))
        {
            _roomMap[position] = new RoomData();
        }
        return _roomMap[position];
    }

    public Vector2Int PlayerCurrentRoom
    {
        get { return _playerCurrentRoom; }
        set { _playerCurrentRoom = value; }
    }
}
