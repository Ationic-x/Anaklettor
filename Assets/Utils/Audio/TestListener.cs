using UnityEngine;

public class TestListener : MonoBehaviour
{
    [SerializeField] public string audioStart = "mus_mm1";
    [Range(0.0f, 6.0f)] public float pitch = 1.0f;
    [Range(0.0f, 1.0f)] public float spatialBlend = 0.0f;
    [Range(0.0f, 1.0f)] public float dopplerLevel = 0.0f;
	[Range(0.0f, 1.0f)] public float volume = 1.0f;
	[SerializeField] public bool loop = true;



	void Start()
    {
        AudioManager.Instance.Play(audioStart, pitch, spatialBlend, dopplerLevel, loop, volume);
    }
}
