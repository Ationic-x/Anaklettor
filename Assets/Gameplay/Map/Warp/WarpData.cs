using UnityEngine;

public class WarpData : MonoBehaviour
{
    private Vector2Int _direction;
	private Vector2Int _newRoom;

	public Vector2Int Direction { get => _direction; set => _direction = value; }
	public Vector2Int NewRoom{ get => _newRoom; set => _newRoom = value; }
}
