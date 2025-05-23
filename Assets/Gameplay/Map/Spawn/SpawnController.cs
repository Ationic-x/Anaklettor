using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public static class SpawnController 
{
	private readonly struct Enemy
	{
		public readonly Vector3 position;
		public readonly Quaternion rotation;

		public Enemy(Vector3 newPosition, Quaternion newRotation)
		{
			position = newPosition;
			rotation = newRotation;
		}
	}

	private static MapManager _mapManager;
	private static GameObject _floor;
	private static GameObject _enemyContainer;
	private static GameObject _map;

	public static void Initialize(MapManager newMapManager, GameObject newFloor, GameObject newEnemyContainer, GameObject newMap)
	{
		_mapManager = newMapManager;
		_floor = newFloor;
		_enemyContainer = newEnemyContainer;
		_map = newMap;
	}

	// Currently using prefab for testing
	public static void SpawnPlayer(GameObject player, ActorData playerData, GameObject playerCamera)
	{
		LevelGenerator levelInstance = _mapManager.LevelInstance;
		Vector3 floorPostion = _floor.transform.position;
		Vector3 floorScale = _floor.transform.localScale;
		Vector2Int dir = levelInstance.MoveDirection;

		Vector3 position = dir == Vector2Int.zero ? new Vector3(floorPostion.x ,floorPostion.y + floorScale.y * 0.5f, floorPostion.z) : levelInstance.PlayerPosition;
		Quaternion rotation = dir == Vector2Int.zero ? new Quaternion(0f, 0f, 0f, 0f) : levelInstance.PlayerRotation;
		Vector3 flip = Vector3.one;

		if (dir == Vector2Int.up || dir == Vector2Int.down)
			flip.z = -0.85f;
		else if (dir == Vector2Int.left || dir == Vector2Int.right)
			flip.x = -0.85f;

		player = Object.Instantiate(player, Vector3.Scale(position, flip), rotation);
		player.GetComponent<PlayerController>().Initialize(playerData);
        //player.GetComponent<BlinkPlayer>().BlinkEffect();

		playerCamera.GetComponent<CinemachineCamera>().Target.TrackingTarget = player.transform;
		Object.Instantiate(playerCamera);
    }

	public static void SpawnEnemies(GameObject enemy, SpawnRules rules, Dictionary<Rol, ActorData> enemiesDictionary)
	{
		if (_mapManager.LevelInstance.TryGetCurrentRoomData(out RoomData roomData))
		{
			while (_enemyContainer.transform.childCount > 0)
				Object.DestroyImmediate(_enemyContainer.transform.GetChild(0).gameObject);

            StoredSpawn(roomData);
		} else
		{
            
			FirstTimeSpawn(enemy, rules, enemiesDictionary);

            _enemyContainer.transform.rotation = _map.transform.rotation;
            _enemyContainer.transform.localScale = _map.transform.localScale;
        }
    }

	private static void StoredSpawn(RoomData roomData)
	{
		foreach (EnemyData enemy in roomData.enemies)
		{
			Object.Instantiate(enemy.actorData.model, enemy.position, enemy.rotation, _enemyContainer.transform);
		}
	}

	private static void FirstTimeSpawn(GameObject enemy, SpawnRules rules, Dictionary<Rol, ActorData> enemiesDictionary)
	{
		Vector3 floorPostion = _floor.transform.position;
		Vector3 floorScale = _floor.transform.localScale;

		// Find enemy container and redefined spawn positions/size (have spawn points)
		List<Enemy> predefinedEnemies = new();

		while (_enemyContainer.transform.childCount > 0)
		{
			Transform child = _enemyContainer.transform.GetChild(0);

			Vector3 enemyPosition = new Vector3(child.position.x, floorPostion.y + floorScale.y * 0.5f, child.position.z);
			Quaternion enemyRotation = Quaternion.Euler(0f, child.eulerAngles.y, 0f);

			predefinedEnemies.Add(new Enemy(enemyPosition, enemyRotation));

			Object.DestroyImmediate(child.gameObject);
		}

		// Randomize roles using the rules
		int min = Mathf.Max(rules.minEnemies, predefinedEnemies.Count);
		int count = Random.Range(min, rules.maxEnemies + 1);

		List<Rol> selectedRoles = new();

		for (int i = 0; i < count; i++)
		{
			// Radom rol or iterative rol
			Rol role = rules.randomizeRoles ? rules.roles[Random.Range(0, rules.roles.Count)] : rules.roles[i % rules.roles.Count];
			selectedRoles.Add(role);
		}

		// Spawn enemies using floor rules and spawnpoints
		List<Enemy> existingEnemies = new(predefinedEnemies);

		// Spawnpoints
		for (int i = 0; i < predefinedEnemies.Count && i < count; i++)
		{
			Vector3 spawnPos = predefinedEnemies[i].position;
			Quaternion spawnRotation = predefinedEnemies[i].rotation;
			GameObject model = enemiesDictionary[selectedRoles[i]].model;

			Object.Instantiate(model, spawnPos, spawnRotation, _enemyContainer.transform);
		}

		// Default behaviour
		for (int i = predefinedEnemies.Count; i < count; i++)
		{
			Vector3 randomPosition = GetValidSpawnPosition(floorPostion, floorScale, rules.spawnAreaRatio, rules.minSpacingBetweenEnemies, existingEnemies);
			if (randomPosition == Vector3.positiveInfinity)
			{
				Debug.LogWarning("No valid position found.");
				continue;
			}

			Quaternion randomYRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

			GameObject enemy2 = Object.Instantiate(enemy, randomPosition, randomYRotation, _enemyContainer.transform);
			enemy2.GetComponent<EnemyController>().Initialize(enemiesDictionary[selectedRoles[i]]);


			existingEnemies.Add(new Enemy(randomPosition, randomYRotation));
		}
	}



	private static Vector3 GetValidSpawnPosition(Vector3 position, Vector3 scale, float spawnAreaRatio, float spacing, List<Enemy> enemies)
	{
		int maxAttempts = 50;

		for (int i = 0; i < maxAttempts; i++)
		{
			float x = Random.Range((position.x - scale.x * 0.5f) * spawnAreaRatio, (position.x + scale.x * 0.5f) * spawnAreaRatio);
			float z = Random.Range((position.z - scale.z * 0.5f) * spawnAreaRatio, (position.z + scale.z * 0.5f) * spawnAreaRatio);
			Vector3 candidate = new Vector3(x, position.y + scale.y * 0.5f, z);

			bool valid = true;
			foreach (var enemy in enemies)
			{
				if (Vector3.Distance(candidate, enemy.position) < spacing)
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
