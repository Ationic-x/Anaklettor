using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public Vector3 position;
    public Quaternion rotation;
    public ActorData actorData;
    public int hp;
    public bool isAlive = true;
    public string name;
}

public class RoomData
{
    public List<EnemyData> enemies = new();
}