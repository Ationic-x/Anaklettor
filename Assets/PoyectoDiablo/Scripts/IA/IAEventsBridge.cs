using UnityEngine;

public class IAEventsBridge : MonoBehaviour
{
    IAController iaController;

    private void Start()
    {
        iaController = transform.GetComponentInChildren<IAController>();

    }

    public void ShootArrow()
    {
        iaController.SpawnArrow();
    }

}