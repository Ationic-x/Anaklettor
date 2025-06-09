using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UIVolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        UIVolumeController(slider.value);
    }

    public void UIVolumeController(float sliderMusica)
    {
        if (sliderMusica == 0)
        {
            audioMixer.SetFloat("UIVolume", -80.0f);
        } else
        {
            audioMixer.SetFloat("UIVolume", Mathf.Log10(sliderMusica) * 20);
        }
    }
}
