using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	[SerializeField] private Material _material;

	public void SetMinimap(LevelGenerator levelInstance)
	{
		_material.SetFloat("_GridSize", levelInstance.MapWidth);
		_material.SetVector("_PlayerCell", new Vector4(levelInstance.PlayerCurrentRoom.x, levelInstance.PlayerCurrentRoom.y, 0, 0));
		_material.SetVectorArray("_VisitedCells", levelInstance.VisitedRooms);
		_material.SetInt("_VisitedCount", levelInstance.VisitedCount);
	}

	public void SetZoom(float zoom)
	{
		_material.SetFloat("_Zoom", Mathf.Clamp01(zoom));
	}
}
