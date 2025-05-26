using System.Collections;
using UnityEngine;

public class ArrowController : MonoBehaviour
{

    private void OnEnable()
    {
        StartCoroutine(lifeTime());
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            //other.GetComponent<PlayerController>().TakeDamage(10);

        }

        Destroy(gameObject);
    }

    IEnumerator lifeTime()
    {
        yield return new WaitForSeconds(5);        
        Destroy(this.gameObject);
    }

}
