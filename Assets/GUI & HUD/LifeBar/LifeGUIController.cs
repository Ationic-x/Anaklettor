using System.Collections;
using UnityEngine;

public class LifeGUIController : MonoBehaviour
{
	[SerializeField] private GameObject _fillHP;
	private PlayerController _player;
	private Animator _animator;

	private readonly float _fillDuration = 0.2f;
	private float _fillScaleX = 1f;

	private Transform _hpTransform;
	private Coroutine _fillCoroutine = null;


	private void Awake()
	{
		_hpTransform = _fillHP.transform;
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


		_player = playerGO.GetComponent<PlayerController>();

		_hpTransform.localScale = new Vector3(HpAtPercentage(), _hpTransform.localScale.y, _hpTransform.localScale.z);
		_player.OnHPChange += UpdateHP;

	}

	private void OnDisable()
	{
		if (_player != null)
			_player.OnHPChange -= UpdateHP;
	}

	private float HpAtPercentage()
	{
		return (float)_player.Stats.HP / (float)_player.Stats.MaxHP;
	}

	public void UpdateHP()
	{

		if (_fillCoroutine != null)
			StopCoroutine(_fillCoroutine);

		_fillCoroutine = StartCoroutine(LerpHP(_hpTransform.localScale.x, HpAtPercentage()));

		if (_player.Stats.HP <= 0)
			_animator.SetTrigger("Out");
	}

	private IEnumerator LerpHP(float start, float end)
	{
		float duration = _fillDuration;
		float t = 0f;

		while (t < 1f)
		{
			_fillScaleX = Mathf.Lerp(start, end, t);

			_hpTransform.localScale = new Vector3(_fillScaleX, _hpTransform.localScale.y, _hpTransform.localScale.z);

			t += Time.deltaTime / duration;
			yield return null; // using null to allow the coroutine work in timescale 0
		}

		yield return new WaitForEndOfFrame();
	}
}