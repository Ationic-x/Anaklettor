using UnityEngine;

[CreateAssetMenu(fileName = "ActorData", menuName = "Scriptable Objects/ActorData")]
public class ActorData : ScriptableObject
{
    public GameObject model;
    public Jobs job;
    public Stats stats;

    public ActorData SetData(GameObject model, Jobs job, Stats stats)
    {
        this.model = model;
        this.job = job;
        this.stats = stats;
        return this;
    }
}
