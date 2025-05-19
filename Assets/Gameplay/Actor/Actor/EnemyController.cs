using UnityEngine;

public class EnemyController : Actor
{
    protected int Level { get; set; }

    void Start()
    {
        MaxHealth = ActorData.stats.MaxHP;
        Health = MaxHealth;
        Mana = ActorData.stats.Mana;
        MaxMana = ActorData.stats.MaxMana;
        AttackPower = ActorData.stats.Attack;
        Defense = ActorData.stats.Defense;
        Level = 1;

        GameObject EnemyRef = Instantiate(ActorData.model, transform.position, Quaternion.identity, transform);

        if (ActorData.name == "Anaklettor")
        {
            EnemyRef.GetComponentInChildren<AnaklettorIAController>().ActorData = ActorData;
        }
        else
        {
            EnemyRef.GetComponentInChildren<IAController>().ActorData = ActorData;
        }

    }

}
