using System;
using UnityEngine;

public class IA_Archer_SoundControllerEventBridge : MonoBehaviour
{

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
    }

    public void ShootingArrow()
    {
        _audioManager.Play3D(SoundsNames.SkeletonShootArrow, this.transform.gameObject, pitch: RandomPitch(1.7f, 2f), volume: 0.66f);
    }

    public void Death()
    {
        _audioManager.Play3D(SoundsNames.SkeletonDeath1, this.transform.gameObject, pitch: RandomPitch(1.5f, 2f), volume: 0.25f);
    }

    public void TakeDamage()
    {
        _audioManager.Play3D(SoundsNames.SkeletonHit, this.transform.gameObject, pitch: RandomPitch(1.2f, 1.8f), playIfAlreadyPlaying: true);
    }

    public void Step()
    {
        _audioManager.Play3D(SoundsNames.SkeletonStep, this.transform.gameObject, pitch: RandomPitch(1f, 2f), volume: 0.5f, playIfAlreadyPlaying: true);
    }

    //FIX Cambiar el sonido por uno mas corto
    public void Search()
    {
        //_audioManager.Play3D(SoundsNames.SkeletonTalk, this.transform.gameObject);
    }


    public float RandomPitch(float min = 0.9f, float max = 1.1f)
    {
        return UnityEngine.Random.Range(min, max);
    }


}
