using System.Collections;
using UnityEngine;

public class GameOverGUIController : MonoBehaviour
{
    private Animator _animator;

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
		_animator.SetTrigger("In");
	}
}
