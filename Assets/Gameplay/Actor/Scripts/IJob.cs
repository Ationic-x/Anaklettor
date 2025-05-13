using UnityEngine;

public interface IJob 
{
    [System.Serializable]
    public enum Jobs
    {
        Knight,
        Mage,
        Rogue,
        Archer,
        Barbarian
    }
    public Jobs Class { get; set; }
}