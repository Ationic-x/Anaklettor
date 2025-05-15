using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    //private Slider slider;

    //private void Awake()
    //{
    //    audioMixer = GetComponent<AudioMixer>();
    //    slider = GetComponent<Slider>();
    //}

    //private void Update()
    //{
    //    MusicController(slider.value);
    //}

    public void MusicController(float sliderMusica)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderMusica) * 20);
    }
}
