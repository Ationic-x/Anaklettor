using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : Actor
{
    [SerializeField] string PlayerName;
    protected int Level { get; set; }
    protected int Experience { get; set; }
    protected int MaxExperience { get; set; }

    void Start()
    {
        PlayerInitialize();
        base.InitializeAnimations();
    }

    void Update()
    {
        
    }
    
    public void PlayerInitialize() 
    {
        MaxHealth = ActorData.stats.MaxHP;
        Health = MaxHealth;
        Mana = ActorData.stats.Mana;
        MaxMana = ActorData.stats.MaxMana;
        AttackPower = ActorData.stats.Attack;
        Defense = ActorData.stats.Defense;
        Experience = 0;
        MaxExperience = 100;
        Level = 1;

        Instantiate(ActorData.model, transform.position, Quaternion.identity, transform);
    }

    void LevelUp() 
    {
        Level++;
        MaxHealth += 10;
        Health = MaxHealth; 
        MaxExperience += 100;
        AttackPower += 3;
        Defense += 5;
    }

    void GainExperience(int amount) 
    {
        Experience += amount;
        if (Experience >= MaxExperience) 
        {
            if (Experience > MaxExperience) 
            {
                Experience = Experience - MaxExperience;
            } 
            else 
            {
                Experience = 0;
            }
            LevelUp();
        }
    }
}
