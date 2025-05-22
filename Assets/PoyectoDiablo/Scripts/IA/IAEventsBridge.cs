using System;
using UnityEngine;

public class IAEventsBridge : MonoBehaviour
{
    public Action isShooting;

    public void ShootArrow()
    {
        isShooting.Invoke();
    }

}