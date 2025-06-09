using System.Collections;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    Animator _animator;

    private AudioManager _audioManager;

    private AnaklettorIAController _controller;

    private void OnEnable()
    {
        StartCoroutine(lifeTime());

        _animator = GetComponent<Animator>();

        _audioManager = FindAnyObjectByType<AudioManager>();

        _controller = FindAnyObjectByType<AnaklettorIAController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.GetComponent<Collider>().enabled = false;

        // Debería estar filtrado por layer gracias al ridgidbody del enemigo, pero por si acaso
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.Hit(transform, _controller.Stats.Attack);

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

    public void SpellExplosion()
    {
        _audioManager.Play3D(SoundsNames.SkeletonSpellExplosion, this.transform.gameObject, pitch: RandomPitch(2f, 7f), volume: 0.5f);
    }

    public float RandomPitch(float min = 0.9f, float max = 1.1f)
    {
        return UnityEngine.Random.Range(min, max);
    }


}
