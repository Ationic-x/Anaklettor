using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Vector3Int position;
    public bool isAlive = true;
}

[System.Serializable]
public class RoomData
{
    public List<EnemyData> enemies = new();
    public bool initialized = false;
}
