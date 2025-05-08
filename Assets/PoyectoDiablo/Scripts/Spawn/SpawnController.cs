using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public static class SpawnController 
{
	public static void SpawnPlayer(MapManager mapManager,GameObject floor, GameObject player)
	{
		LevelGenerator levelInstance = mapManager.LevelInstance;
		Vector2Int dir = levelInstance.MoveDirection;

		Vector3 position = dir == Vector2Int.zero ? Vector3.zero : levelInstance.PlayerPosition;
		Quaternion rotation = dir == Vector2Int.zero ? new Quaternion(0f, 180f, 0f, 0f) : levelInstance.PlayerRotation;
		Vector3 flip = Vector3.one;

		if (dir == Vector2Int.up || dir == Vector2Int.down)
			flip.z = -0.9f;
		else if (dir == Vector2Int.left || dir == Vector2Int.right)
			flip.x = -0.9f;

		player = Object.Instantiate(player, Vector3.Scale(position, flip), rotation);
		player.GetComponentInChildren<TestMovement>().SetMapManager(mapManager);
	}

	public static void SpawnEnemies(SpawnRules rules, GameObject floor, Dictionary<Rol, GameObject> enemiesDictionary)
	{
		Vector3 floorPostion = floor.transform.position;
		Vector3 floorScale = floor.transform.localScale;

		// Find enemy container and redefined spawn positions/size (have spawn points)
		GameObject enemyContainer = GameObject.FindGameObjectWithTag(Tags.Floor);
		List<Vector3> predefinedPositions = new();
		
		for (int i = 0; i < enemyContainer.transform.childCount; i++)
		{
			Transform child = enemyContainer.transform.GetChild(i);
			predefinedPositions.Add(new Vector3(child.position.x, floorPostion.y + floorScale.y * 0.5f, child.position.z));
		}
		int min = Mathf.Max(rules.minEnemies, predefinedPositions.Count);
		int count = Random.Range(min, rules.maxEnemies + 1);

		// Randomize roles using the rules
		List<Rol> selectedRoles = new();

		if (rules.randomizeRoles)
		{
			for (int i = 0; i < count; i++)
			{
				Rol randomRole = rules.roles[Random.Range(0, rules.roles.Count)];
				selectedRoles.Add(randomRole);
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				selectedRoles.Add(rules.roles[i % rules.roles.Count]);
			}
		}

		// Spawn enemies using floor rules and spawpoints
		List<Vector3> usedPositions = new(predefinedPositions);

		// Spawnpoints
		for (int i = 0; i < predefinedPositions.Count && i < count; i++)
		{
			Vector3 spawnPos = predefinedPositions[i];
			Quaternion randomYRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			GameObject prefab = enemiesDictionary[selectedRoles[i]];
			Object.Instantiate(prefab, spawnPos, randomYRotation, enemyContainer.transform);
		}

		// Default behaviour
		for (int i = predefinedPositions.Count; i < count; i++)
		{
			Vector3 spawnPos = GetValidSpawnPosition(floorPostion, floorScale, rules.minDistanceFromEdge, rules.minSpacingBetweenEnemies, usedPositions);
			if (spawnPos == Vector3.positiveInfinity)
			{
				Debug.LogWarning("No valid position found.");
				continue;
			}

			Quaternion randomYRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			GameObject prefab = enemiesDictionary[selectedRoles[i]];
			GameObject enemy = Object.Instantiate(prefab, spawnPos, randomYRotation, enemyContainer.transform);
			usedPositions.Add(spawnPos);
		}
	}

	private static Vector3 GetValidSpawnPosition(Vector3 position, Vector3 scale, float edgeMargin, float spacing, List<Vector3> existing)
	{
		int maxAttempts = 50;

		for (int i = 0; i < maxAttempts; i++)
		{
			float x = Random.Range(position.x - scale.x * 0.5f + edgeMargin, position.x + scale.x * 0.5f - edgeMargin);
			float z = Random.Range(position.z - scale.z * 0.5f + edgeMargin, position.z + scale.z * 0.5f - edgeMargin);
			Vector3 candidate = new Vector3(x, position.y + scale.y * 0.5f, z);

			bool valid = true;
			foreach (var pos in existing)
			{
				if (Vector3.Distance(candidate, pos) < spacing)
				{
					valid = false;
					break;
				}
			}

			if (valid)
				return candidate;
		}

		return Vector3.positiveInfinity; // fallback
	}
}
