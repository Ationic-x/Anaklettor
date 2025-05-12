using UnityEngine;

public class MinimapRenderer : MonoBehaviour
{
	[SerializeField] private Material _material;
	private bool _isInitialized = false;
	private const int _MaxVisited = 64; // This const is linked to the shader (should avoid hardcode?)
	private ArrayQueue<Vector4> _visitedRooms = new(_MaxVisited, new Vector4(-1, -1, 0, 0));

	public void InitMinimap(float InitialSize, Vector2 InitialPosition)
	{
		if (_isInitialized) return;

		_isInitialized = true;
		SetSize(InitialSize);
		SetPlayerCell(InitialPosition);
	}

	public void MovePlayer(Vector2 direction)
	{
		Vector2 currentPosition = _material.GetVector("_PlayerRoom");
		SetPlayerCell(currentPosition + direction);
	}

	public void SetPlayerCell(Vector2 position)
	{
		_visitedRooms.PushHash(new Vector4(position.x, position.y, 0, 0)); // Store previous value without repeating
		_material.SetVector("_PlayerRoom", new Vector4(position.x, position.y, 0, 0));
	}

	public void SetSize(float size)
	{
		_material.SetFloat("_MapSize", size);
	}

	// This method is used to hardset or override values of minimap
	// Recommended method when scene reload and you dont have the shader in a singleton
	// Or this class is not a singleton
	public void SetMinimap(LevelGenerator levelInstance)
	{
		SetSize(levelInstance.MapWidth);
		SetPlayerCell(levelInstance.PlayerCurrentRoom);
		_visitedRooms = levelInstance.VisitedRooms;
		SetVisitedRoom();
	}

	public void SetZoom(float zoom)
	{
		_material.SetFloat("_Zoom", Mathf.Clamp01(zoom));
	}

	public void SetVisitedRoom()
	{
		_material.SetInt("_VisitedCount", _visitedRooms.Count);
		_material.SetVectorArray("_VisitedRooms", _visitedRooms.ToArray());
	}
}
