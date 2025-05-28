using System;
using UnityEngine;

public class IAEventsBridge : MonoBehaviour
{
    public Action isShooting;

    public Action spellThrow;

    public void ShootArrow()
    {
        isShooting.Invoke();
    }

    public void ThrowSpell()
    {
        spellThrow.Invoke();        
    }

}