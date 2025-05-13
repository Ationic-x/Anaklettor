using UnityEngine;

public class TestEmitter : MonoBehaviour
{
    [SerializeField] public string audioStart = "snd_chara_atk";
	[Range(0.0f, 6.0f)] public float pitch = 1.0f;
	[Range(0.0f, 1.0f)] public float spatialBlend = 1.0f;
	[Range(0.0f, 1.0f)] public float dopplerLevel = 0.0f;
	[Range(0.0f, 1.0f)] public float volume = 1.0f;
	[SerializeField] public bool loop = false;
	void Start()
    {
        AudioManager.Instance.Play3D(audioStart, gameObject, pitch, spatialBlend, dopplerLevel, loop, volume);
    }
}
