using System.Collections;
using UnityEngine;

public class SoulController : MonoBehaviour
{
	[SerializeField] private float _delay;

	private int _exp = 100;
	private Coroutine _searchPlayer;

	private void OnEnable()
	{
		_searchPlayer = StartCoroutine(SearchForPlayer(_delay));
	}

	IEnumerator SearchForPlayer(float delay)
	{
		yield return new WaitForSecondsRealtime(delay);

		GameObject player = GameObject.FindGameObjectWithTag(Tags.Player);
		Vector2 src = new Vector2(transform.position.x, transform.position.z);
		Vector2 dst = new Vector2(player.transform.position.x, player.transform.position.z);

		while (Vector2.Distance(src, dst) > 1f)
		{
			Vector3 move = Vector3.MoveTowards(
				transform.position,
				player.transform.position,
				6f * Time.unscaledDeltaTime
			);

			transform.position = new Vector3 (move.x, 2, move.z);

			src = new Vector2(transform.position.x, transform.position.z);
			dst = new Vector2(player.transform.position.x, player.transform.position.z);

			yield return null;
		}

		player.GetComponent<PlayerController>().GainExperience(_exp);
		Destroy(gameObject);
	}

	public void ForcedSearchPlayer()
	{
		if (_searchPlayer != null)
			StopCoroutine(_searchPlayer);

		_searchPlayer = StartCoroutine(SearchForPlayer(0));
	}

	public void SetAmountExp(int amount)
	{
		_exp = amount;
	}
}
