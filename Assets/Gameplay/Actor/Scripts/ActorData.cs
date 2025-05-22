using UnityEngine;

[CreateAssetMenu(fileName = "ActorData", menuName = "Scriptable Objects/ActorData")]
public class ActorData : ScriptableObject
{
    public GameObject model;
    public Jobs job;
    public string nickname;
    public Stats stats;
}
