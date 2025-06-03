using UnityEngine;
using UnityEngine.UI;

public class UIAudioTrigger : MonoBehaviour
{
    public Button button;

    public int selectedIndex;

    private string[] soundsNames = {
       SoundsNames.UIStartGame,
       SoundsNames.UIConfirm,
       SoundsNames.UICancel,
       SoundsNames.UIMove,
   };

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        if (button != null && button.onClick != null)
        {
            button.onClick.AddListener(PlayAudio);
        }
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(PlayAudio);
    }

    private void PlayAudio()
    {
        AudioManager.Instance.Play(soundsNames[selectedIndex], loop: false, volume: 0.22f);
    }
}
