using UnityEngine;
using System.Collections;

public class ChangeMaterialColor : MonoBehaviour
{
	[SerializeField] private float _blinkDuration = 0.1f;
	[SerializeField] private int _blinkCount = 2;
	[SerializeField] private Color _blinkColor = new Color(1f, 0f, 0f, 1f);
	[SerializeField] private Color _alternativeColor = new Color(0f, 0f, 1f, 1f);
	[SerializeField] private Material _materialRef;

	public void TriggerBlink()
	{
		StopAllCoroutines();
		StartCoroutine(BlinkCoroutine());
	}

	public void SetColorSeconds(float seconds)
	{
		StartCoroutine(SetColorCoroutine(seconds));
	}

	private IEnumerator BlinkCoroutine()
	{
		for (int i = 0; i < _blinkCount; i++)
		{
			SetColor(_blinkColor);
			yield return new WaitForSeconds(_blinkDuration);
			SetColor(Color.white);
			yield return new WaitForSeconds(_blinkDuration);
		}
	}

	private IEnumerator SetColorCoroutine(float seconds)
	{
		SetColor(_alternativeColor);
		yield return new WaitForSeconds(seconds);
		SetColor(Color.white);
	}

	private void SetColor(Color color)
	{
		_materialRef.SetColor("_BaseColor", color);
	}

}
