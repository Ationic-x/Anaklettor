using System.Collections;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    Animator _animator;

    [SerializeField] private int _spellDamage = 0;

    public int SpellDamage { get => _spellDamage; set => _spellDamage = value; }

    private void OnEnable()
    {
        StartCoroutine(lifeTime());

        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.GetComponent<Collider>().enabled = false;

        // Debería estar filtrado por layer gracias al ridgidbody del enemigo, pero por si acaso
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.Hit(transform, SpellDamage);
            transform.GetComponent<Rigidbody>().isKinematic = true;

            _animator.SetBool("ExplotionTrigger", true);

            Destroy(this.gameObject, 5f);
        }
    }

    IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(5);
        _animator.SetBool("ExplotionTrigger", true);
        Destroy(this.gameObject, 5f);
    }

}
