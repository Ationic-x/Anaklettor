using UnityEngine;

public interface IActor
{
    void TakeDamage(int amount);
    void Heal(int amount);
    void Move();
    void Attack();
    void Rotate();
    void Interact();
}
