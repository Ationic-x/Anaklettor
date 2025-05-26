using System.Collections;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    Animator _animator;

    private void OnEnable()
    {
        StartCoroutine(lifeTime());

        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.GetComponent<Collider>().enabled = false;

        if (other.tag == "Player")
        {
            //Ejecutar metodo de hacer daño al jugador
        }

        transform.GetComponent<Rigidbody>().isKinematic = true;

        _animator.SetBool("ExplotionTrigger", true);

        Destroy(this.gameObject, 5f);
    }

    IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(5);
        _animator.SetBool("ExplotionTrigger", true);
        Destroy(this.gameObject, 5f);
    }

}
