using UnityEngine;
using System.Collections;

public class DamageBlink : MonoBehaviour
{
	[SerializeField] private float _blinkDuration = 0.1f;
	[SerializeField] private int _blinkCount = 2;
	[SerializeField] private Color _blinkColor = new Color(1f, 0f, 0f, 1f);
	[SerializeField] private Material _materialRef;

	public void TriggerBlink()
	{
		StopAllCoroutines();
		StartCoroutine(BlinkCoroutine());
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

	private void SetColor(Color color)
	{
		_materialRef.SetColor("_BaseColor", color);
	}

}
