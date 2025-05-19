using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXVolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        SFXVolumeController(slider.value);
    }

    public void SFXVolumeController(float sliderMusica)
    {
        if (sliderMusica == 0)
        {
            audioMixer.SetFloat("SFXVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sliderMusica) * 20);
        }
    }
}
