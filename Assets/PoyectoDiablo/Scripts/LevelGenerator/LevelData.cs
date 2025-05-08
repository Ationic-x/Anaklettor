using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Vector3 position;
    public Quaternion rotation;
    public GameObject prefab; // Currently hardcode should be a Scriptable Object
    public int hp;
    public bool isAlive = true;
    public string name;
}

[System.Serializable]
public class RoomData
{
    public List<EnemyData> enemies = new();
    public bool initialized = false;
}