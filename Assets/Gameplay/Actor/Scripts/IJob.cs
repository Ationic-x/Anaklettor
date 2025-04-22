using UnityEngine;

public interface IJob 
{
    [System.Serializable]
    public enum Jobs
    {
        Warrior,
        Mage,
        Rogue,
        Barbarian
    }
    public Jobs Class { get; set; }
}