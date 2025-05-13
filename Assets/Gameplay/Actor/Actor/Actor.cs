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

    protected Rigidbody rb;

    protected float movementSpeed;
    protected Vector3 eulerAngleVelocity;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
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

    public void Move()
    {
        // rb.linearVelocity = direction * ActorData.stats.Speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + (transform.forward * movementSpeed * Time.fixedDeltaTime));
    }

    public void Rotate()
    {
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }
}
