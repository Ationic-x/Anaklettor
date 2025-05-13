using UnityEngine;
using UnityEngine.Rendering;

public class TestEmitter2 : MonoBehaviour
{
    [SerializeField] public string audioStart = "snd_skeleton_talk";
	[Range(0.0f, 6.0f)] public float pitch = 1.0f;
	[Range(0.0f, 1.0f)] public float spatialBlend = 0.0f;
	[Range(0.0f, 1.0f)] public float dopplerLevel = 0.0f;
	[Range(0.0f, 1.0f)] public float volume = 1.0f;
	[SerializeField] public bool loop = false;
	void Start()
	{
        AudioManager.Instance.PlayAtPoint(audioStart, gameObject.transform.position, pitch, spatialBlend, dopplerLevel, loop, volume);
    }
}