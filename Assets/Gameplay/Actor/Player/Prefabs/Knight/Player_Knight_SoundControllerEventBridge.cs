using System;
using UnityEngine;

public class Player_Knight_SoundControllerEventBridge : MonoBehaviour
{

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
    }

    public void Talk()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightVTalk, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void Attack()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightVAttack, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void ChargedAttack()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightVCharged, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void Fanfarre()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightVFanfarre, this.transform.gameObject, pitch: RandomPitch(0.9f, 1.1f), volume: 0.33f);
    }

    public void SwordSlash()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightSlash, this.transform.gameObject, pitch: RandomPitch(1f, 2f), volume: 0.5f);
    }

    public void SwordBlow()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightBlow, this.transform.gameObject, pitch: RandomPitch(1f, 2f), volume: 0.5f);
    }
    public void Block()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightBlock, this.transform.gameObject, pitch: RandomPitch(1f, 2f), volume: 0.5f);
    }

    public void Death()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightDeath1, this.transform.gameObject, pitch: RandomPitch(0.5f, 0.7f), volume: 0.25f);
    }

    public void TakeDamage()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightHit, this.transform.gameObject, pitch: RandomPitch(1.2f, 2f), playIfAlreadyPlaying: true);
    }

    public void StepLeft()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightStepLeft, this.transform.gameObject, pitch: RandomPitch(1.5f, 2f), volume: 0.5f, playIfAlreadyPlaying: true);
    }
    public void StepRight()
    {
        if (!_audioManager) return;
        _audioManager.Play3D(SoundsNames.KnightStepRight, this.transform.gameObject, pitch: RandomPitch(1.5f, 2f), volume: 0.5f, playIfAlreadyPlaying: true);
    }

    public float RandomPitch(float min = 0.9f, float max = 1.1f)
    {
        return UnityEngine.Random.Range(min, max);
    }


}
