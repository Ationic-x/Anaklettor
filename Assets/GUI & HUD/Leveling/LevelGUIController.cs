using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _optionsUI;
    [SerializeField] private SerializableDictionary<Texture2D, string> _listUpgrades;

    private List<KeyValuePair<Texture2D, string>> _kvpUpgrades;
	private PlayerController _player;
	private Animator _animator;
	private float _previousTimeScale;

	public Action<bool> OnLevelUpAction;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		_kvpUpgrades = _listUpgrades.ToDictionary().ToList();
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

		_player.OnLevelUp += OnLevelUP;
		_player.OnUpgradeSelected += OnUpgradeSelected;
	}

	private void OnDisable()
	{
		if (_player != null)
		{
			_player.OnLevelUp -= OnLevelUP;
			_player.OnUpgradeSelected -= OnUpgradeSelected;
		}
	}

	private void OnLevelUP()
	{
		_previousTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		OnLevelUpAction.Invoke(true);

		HashSet<int> choosenOptions = new HashSet<int>();
        foreach (GameObject option in _optionsUI)
		{
			Button button = option.GetComponent<Button>();
			button.interactable = false;

			int co;
            do
            {
                co = UnityEngine.Random.Range(0, _kvpUpgrades.Count);
            } while (choosenOptions.Contains(co));

            choosenOptions.Add(co);


		    option.GetComponentInChildren<RawImage>().texture = _kvpUpgrades[co].Key;
			option.GetComponentInChildren<TextMeshProUGUI>().text = _kvpUpgrades[co].Value;

			

			// We use the name of the icon as the key for the upgrade
			switch (_kvpUpgrades[co].Key.name)
			{
				case "HigherAspd":
					button.onClick.AddListener(() => _player.IncreaseAspd());
					break;
				case "HigherBspd":
					button.onClick.AddListener(() => _player.IncreaseAspd());
					break;													
				case "HigherAtk":
					button.onClick.AddListener(() => _player.IncreaseAtk());
					break;													
				case "HigherHP":
					button.onClick.AddListener(() => _player.IncreaseHP());
					break;													
				case "HigherInvTime":
					button.onClick.AddListener(() => _player.IncreaseImmunity());
					break;													
				case "HigherSpeed":
					button.onClick.AddListener(() => _player.IncreaseSpeed());
					break;													
				case "LowerAtkCD":
					button.onClick.AddListener(() => _player.LowerAtkCD());
					break;													
				default:
					button.onClick.AddListener(() => Debug.Log("Unknown upgrade"));
					break;
			}
        }

		StartCoroutine(WaitForEnter());
	}

	private void OnUpgradeSelected()
	{
		foreach (GameObject option in _optionsUI)
		{
			option.GetComponent<Button>().interactable = false;
		}

		StartCoroutine(WaitForExit());
	}

	IEnumerator WaitForEnter()
	{
		_animator.SetTrigger("In");
		yield return new WaitForEndOfFrame();
		// Added small delay to ensure player does not click too fast
		yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.2f);

		foreach (GameObject option in _optionsUI)
		{
			option.GetComponent<Button>().interactable = true;
		}
	}

	IEnumerator WaitForExit()
	{
		_animator.SetTrigger("Out");
		yield return new WaitForEndOfFrame();
		yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + 0.5f);

		Time.timeScale = _previousTimeScale;
		OnLevelUpAction.Invoke(false);
	}
}
