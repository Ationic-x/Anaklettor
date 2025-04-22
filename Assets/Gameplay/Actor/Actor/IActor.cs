using UnityEngine;

public interface IActor
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void Move(Vector3 direction);
    void Attack();
    void Rotate(Vector3 targetDirection);
    void Interact();
}
