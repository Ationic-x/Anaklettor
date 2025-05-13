using UnityEngine;

[System.Serializable]
public class Job : IJob
{
    [SerializeField] protected IJob.Jobs jobType;

    public IJob.Jobs Class
    {
        get => jobType;
        set => jobType = value;
    }
}