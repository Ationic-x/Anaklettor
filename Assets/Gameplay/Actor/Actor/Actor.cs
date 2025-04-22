using UnityEngine;

public class Actor : MonoBehaviour, IActor
{
    public ActorDataSO ActorData;
    protected int Health;
    protected int MaxHealth;
    protected int Mana;
    protected int MaxMana;
    protected int AttackPower;
    protected int Defense;
    protected GameObject actorModelObject;

    public void InitializeAnimations()
    {
        GameObject actorModelObject = gameObject.transform.GetChild(0).gameObject;
        actorModelObject.AddComponent<Animator>().runtimeAnimatorController = ActorData.animatorController;
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public void Move(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public void Rotate(Vector3 targetDirection)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }
}
