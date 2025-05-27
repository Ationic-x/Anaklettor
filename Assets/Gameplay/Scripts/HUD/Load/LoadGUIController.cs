using System.Collections;
using TMPro;
using UnityEngine;

public class LoadGUIController : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
	{
		_animator = GetComponent<Animator>();
		GetComponentInChildren<TextMeshProUGUI>().text = "Cargando..."; // In case of multiple languages
	}

	public void LoadAnimationDirection(Vector2Int direction)
	{
		string animationDirection = direction == Vector2Int.up ? "Top" :
						direction == Vector2Int.down ? "Bottom" :
						direction == Vector2Int.left ? "Left" :
						"Right";

		_animator.SetTrigger(animationDirection);
	}

	public void Enable()
	{
		_animator.SetTrigger("Enable");
	}

	public float GetClipDuration()
	{
		return _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
	}
}
