using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class GameOverGUIController : MonoBehaviour
{
    private Animator _animator;

    public Action<bool> OnEnable;

    [SerializeField] Selectable initButton;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(WaitPlayer());
    }

    private IEnumerator WaitPlayer()
    {
        GameObject playerGO;
        do
        {
            playerGO = GameObject.FindGameObjectWithTag(Tags.Player);
            yield return null;
        } while (playerGO == null);

        playerGO.GetComponent<PlayerController>().OnPlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        initButton.Select();
        _animator.SetTrigger("In");
        OnEnable.Invoke(true);
    }
}
