using UnityEngine;

public interface IJob 
{
    [System.Serializable]
    public enum Jobs
    {
        Knight,
        Mage,
        Rogue,
        Barbarian
    }
    public Jobs Class { get; set; }
}