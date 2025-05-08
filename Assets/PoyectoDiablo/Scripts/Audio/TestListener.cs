using UnityEngine;

public class TestListener : MonoBehaviour
{
    [SerializeField] public string audioStart = "mus_mm1";

    void Start()
    {
        AudioManager.Instance.Play(audioStart);
    }
}
