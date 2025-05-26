using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public static class SaveController
{
    private static LevelGenerator _levelInstance;

	public static void Initialize(LevelGenerator newLevelInstance)
    {
        _levelInstance = newLevelInstance;
    }

	// This method is used to save all the enemies in the current room
	// We're currently hardcoding the actor data but should be gotten from the enemycontroller or with other methods
	public static void SaveEnemies()
    {
		bool firstTime = false;
		GameObject enemyContainer = GameObject.FindGameObjectWithTag(Tags.Spawn);
		RoomData roomData;


		if (!_levelInstance.TryGetCurrentRoomData(out roomData))
		{
			firstTime = true;
			roomData = _levelInstance.NewRoomData(_levelInstance.PlayerCurrentRoom);
		}

		// First element of child count is always the gameobject (itself)
		for (int i = 0; i < enemyContainer.transform.childCount; i++)
		{
            Transform child = enemyContainer.transform.GetChild(i);
			EnemyController enemy =  child.GetComponent<EnemyController>();

			if (firstTime)
			{
				EnemyData enemyData = new EnemyData();

				enemyData.position = child.position;
				enemyData.rotation = child.rotation;

				enemyData.actorData = ScriptableObject.CreateInstance<ActorData>().SetData(enemy.Model, enemy.Job, enemy.Stats);
				enemyData.name = enemy.Name;

				roomData.enemies.Add(enemyData);
			}
			else
			{
				EnemyData enemyData = roomData.enemies[i];

				enemyData.position = child.position;
				enemyData.rotation = child.rotation;

				enemyData.actorData.SetData(enemy.Model, enemy.Job, enemy.Stats);
			}
		}
    }

    public static void SavePlayer(Vector2Int newRoom, Vector2Int direction)
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);
        _levelInstance.PlayerCurrentRoom = newRoom;
        _levelInstance.MoveDirection = direction;
        _levelInstance.PlayerPosition = player.transform.position;
        _levelInstance.PlayerRotation = player.transform.rotation;
    }
}
