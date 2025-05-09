using UnityEngine;

public static class WarpGenerator
{
	public static void Generate(LevelGenerator levelInstance, GameObject floor, GameObject warpPrefab)
	{
		GameObject warpContainer = GameObject.FindGameObjectWithTag(Tags.Warp);
		if (warpContainer == null)
		{
			Debug.LogError("Warp container not found");
			return;
		}

		float warpScaleX = floor.transform.localScale.x * 0.1f;
		float warpScaleZ = floor.transform.localScale.z * 0.01f;
		float floorYOffset = floor.transform.localScale.y * 0.55f;
		float marginOffset = (floor.transform.localScale.x - warpScaleX) * 0.5f;

		Vector2Int[] directions = {
			Vector2Int.up,
			Vector2Int.down,
			Vector2Int.right,
			Vector2Int.left
		};

		float[] rotations = { 180f, 0f, 270f, 90f };

		for (int i = 0; i < directions.Length; i++)
		{
			Vector2Int dir = directions[i];
			Vector2Int newRoom = levelInstance.PlayerCurrentRoom + dir;
			float rotationY = rotations[i];

			if (!levelInstance.IsValid(newRoom)) continue;

			Vector3 offset = dir == Vector2Int.up || dir == Vector2Int.down
				? new Vector3(0f, floorYOffset, marginOffset * dir.y)
				: new Vector3(marginOffset * dir.x, floorYOffset, 0f);

			Vector3 warpPosition = floor.transform.position + offset;

			GameObject warpInstance = GameObject.Instantiate(warpPrefab, warpPosition, Quaternion.Euler(0f, rotationY, 0f), warpContainer.transform);
			WarpData data = warpInstance.GetComponentInChildren<WarpData>();
			data.Direction = dir;
			data.NewRoom = newRoom;

			warpInstance.transform.localScale = new Vector3(warpScaleX, warpInstance.transform.localScale.y, warpScaleZ);
		}
	}
}