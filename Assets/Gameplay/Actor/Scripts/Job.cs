using UnityEngine;

[System.Serializable]
public class Job : IJob
{
    [SerializeField] protected IJob.Jobs jobType;
    public IJob.Jobs Class { 
        get => Class; 
        set => Class = value; 
    }
}