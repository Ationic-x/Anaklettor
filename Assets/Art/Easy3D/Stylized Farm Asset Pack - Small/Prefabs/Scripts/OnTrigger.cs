using UnityEngine;
using UnityEngine.Events;

public class OnTrigger : MonoBehaviour
{
    public UnityEvent onPlayerEnter;
    public UnityEvent onPlayerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            onPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(Tags.Player))
        {
            onPlayerExit.Invoke();
        }


    }
}
