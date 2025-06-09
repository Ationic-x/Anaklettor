using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MasterVolumeControl : MonoBehaviour
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
            audioMixer.SetFloat("MasterVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMusica) * 20);
        }
    }
}
