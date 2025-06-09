using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class VictoryGUIController : MonoBehaviour
{
    private Animator _animator;
    private CinemachineCamera _mapCamera;

    public Action<bool> OnEnable;

    [SerializeField] Selectable initButton;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _mapCamera = GameObject.Find("Map Camera").GetComponent<CinemachineCamera>();
    }

    public void Initialize(AnaklettorIAController Anaklettor)
    {
        Anaklettor.OnDead += OnBossDead;
    }

    private void OnBossDead()
    {
        _mapCamera.Priority = 30;

        initButton.Select();
        _animator.SetTrigger("In");
        OnEnable.Invoke(true);
    }


}
