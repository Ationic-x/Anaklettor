using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public static class SaveController
{
    private static LevelGenerator _levelInstance;

    public static void Initialize(MapManager mapManager)
    {
        _levelInstance = mapManager.LevelInstance;
    }

    public static void SaveEnemies(GameObject prefab)
    {
        RoomData roomData = _levelInstance.GetOrCreateCurrentRoomData();
        GameObject enemyContainer = GameObject.FindGameObjectWithTag(Tags.Spawn);

        if (roomData.initialized == false)
        {
            for (int i = 0; i < enemyContainer.transform.childCount; i++)
            {
                var enemyData = new EnemyData();
                enemyData.position = enemyContainer.transform.GetChild(i).position;
                enemyData.rotation = enemyContainer.transform.GetChild(i).rotation;
                enemyData.prefab = prefab; // hardcode should be a Scriptable Object obtained from the component
                // From the scriptable object we will get max hp
                roomData.enemies.Add(enemyData);
            }

            roomData.initialized = true;
        }
        else
        {
            for (int i = 0; i < enemyContainer.transform.childCount; i++)
            {
                roomData.enemies[i].position = enemyContainer.transform.GetChild(i).position;
                roomData.enemies[i].rotation = enemyContainer.transform.GetChild(i).rotation;
            }
        }
    }

    public static void SaveEnemy()// Receive Scriptable Object
    {
        RoomData roomData = _levelInstance.GetOrCreateCurrentRoomData();
        foreach (EnemyData enemyData in roomData.enemies)
        {
            //enemyData.name.Equals()// Scriptable Object Name
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
