using System.Collections;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private int _arrowDamage = 0;

    public int ArrowDamage { get => _arrowDamage; set => _arrowDamage = value; }

    private void OnEnable()
    {
        StartCoroutine(lifeTime());
    }

    private void OnTriggerEnter(Collider other)
    {

        // Debería estar filtrado por layer gracias al ridgidbody del enemigo, pero por si acaso
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {                        
            player.Hit(transform, ArrowDamage);
            Destroy(gameObject);
        }
        
    }

    IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

}
