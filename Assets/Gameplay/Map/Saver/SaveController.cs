using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public static class SaveController
{
    private static LevelGenerator _levelInstance;

	// Unique ID in a funny way
	// The ammount of names should be bigger than the ammount of enemies

	public static void Initialize(LevelGenerator newLevelInstance)
    {
        _levelInstance = newLevelInstance;
    }

	// This method is used to save all the enemies in the current room
	// We're currently hardcoding the actor data but should be gotten from the enemycontroller or with other methods
	public static void SaveEnemies(ActorDataSO actorData)
    {
		bool firstTime = false;
		GameObject enemyContainer = GameObject.FindGameObjectWithTag(Tags.Spawn);
		RoomData roomData;



		if (!_levelInstance.TryGetCurrentRoomData(out roomData))
		{
			firstTime = true;
			roomData = _levelInstance.NewRoomData(_levelInstance.PlayerCurrentRoom);
		}

		for (int i = 0; i < enemyContainer.transform.childCount; i++)
		{
			if (firstTime)
			{
				var enemyData = new EnemyData();
				enemyData.position = enemyContainer.transform.GetChild(i).position;
				enemyData.rotation = enemyContainer.transform.GetChild(i).rotation;
				enemyData.actorData = actorData;
				enemyData.name = EnemyNames.enemyNameID[i];
				roomData.enemies.Add(enemyData);
			}
			else
			{
				roomData.enemies[i].position = enemyContainer.transform.GetChild(i).position;
				roomData.enemies[i].rotation = enemyContainer.transform.GetChild(i).rotation;
				// Should be stored the hp also
				// Should be store if is alive also
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
