using System.Collections;
using UnityEngine;

public class BlinkPlayer : MonoBehaviour
{
    private Animator _blinkAnimator;

    public void Awake()
    {
        _blinkAnimator = GetComponent<Animator>();
    }

    public void BlinkEffect()
    {
        _blinkAnimator.SetTrigger("Blink");
    }
}
