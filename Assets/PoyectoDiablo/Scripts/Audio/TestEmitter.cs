using UnityEngine;

public class TestEmitter : MonoBehaviour
{
    [SerializeField] public string audioStart = "snd_chara_atk";

    void Start()
    {
        AudioManager.Instance.Play3D(audioStart, gameObject);
    }
}
