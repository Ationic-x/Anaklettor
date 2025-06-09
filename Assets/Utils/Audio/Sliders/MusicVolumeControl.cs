using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        MusicVolumeController(slider.value);
    }

    public void MusicVolumeController(float sliderMusica)
    {
        if (sliderMusica == 0)
        {
            audioMixer.SetFloat("MusicVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(sliderMusica) * 20);
        }
    }
}
