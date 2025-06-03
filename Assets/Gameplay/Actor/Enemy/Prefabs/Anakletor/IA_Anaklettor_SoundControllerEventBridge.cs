using System;
using UnityEngine;

public class IA_Anaklettor_SoundControllerEventBridge : MonoBehaviour
{

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
    }

    public void Talk()
    {
        _audioManager.Play3D(SoundsNames.AnakletorTalk, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void Complain1()
    {
        _audioManager.Play3D(SoundsNames.AnakletorComplain1, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void Complain2()
    {
        _audioManager.Play3D(SoundsNames.AnakletorComplain2, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void ShootingSpell()
    {
        _audioManager.Play3D(SoundsNames.AnakletorShootSpell, this.transform.gameObject, pitch: RandomPitch(2f, 2.7f));
    }

    public void RechargingSpell()
    {
        _audioManager.Play3D(SoundsNames.SkeletonRechargingSpell, this.transform.gameObject, pitch: RandomPitch(2f, 2.7f));
    }

    public void Death()
    {
        _audioManager.Play3D(SoundsNames.AnakletorDeath, this.transform.gameObject, pitch: RandomPitch(1.5f, 2f), volume: 0.25f);
    }

    public void TakeDamage()
    {
        _audioManager.Play3D(SoundsNames.AnakletorHit, this.transform.gameObject, pitch: RandomPitch(1.5f, 1.8f), playIfAlreadyPlaying: true);
    }

    public void Step()
    {
        _audioManager.Play3D(SoundsNames.AnakletorStep, this.transform.gameObject, pitch: RandomPitch(1.5f, 2f), volume: 0.5f, playIfAlreadyPlaying: true);
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
