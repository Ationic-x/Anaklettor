using UnityEngine;

public class TestEmitter2 : MonoBehaviour
{
    [SerializeField] public string audioStart = "snd_skeleton_talk";

    void Start()
    {
        AudioManager.Instance.PlayAtPoint(audioStart, gameObject.transform.position);
    }
}