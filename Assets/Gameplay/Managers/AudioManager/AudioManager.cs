using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public static class AudioMathHelper {
    public static float ToDecibels(float v) {
        if (v <= 0) return -80f;
        return (float)(Math.Log10(v) * 20);
	}
}

/// <summary>
/// AudioManager gestiona la reproducción de sonidos en el juego.
/// Se encarga de inicializar un diccionario global de sonidos a partir de múltiples bibliotecas de sonidos (Instancias de SoundLib).
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

	[Header("Volume")]
	[Range(0, 1)] public float masterVolume;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float sfxVolume;
    [Range(0, 1)] public float uiVolume;

    [Header("AudioMixers")]
    public AudioMixer audioMixer;
    public AudioMixerGroup musicAudioGroup;
    public AudioMixerGroup sfxAudioGroup;
    public AudioMixerGroup uiAudioGroup;

    [Header("Sliders")]
    public Slider masterSlider;
	public Slider musicSlider;
	public Slider sfxSlider;
	public Slider uiSlider;

	[Tooltip("List of SoundLibs")]
    public List<SoundLib> soundLibsList;

    private Dictionary<string, (AudioClip clip, SoundCategory category)> globalSoundDictionary;

    private AudioSource musicSource;
    private string currentMusicName = null;

    /// <summary>
    /// Inicializa el AudioSource y construye el diccionario global de sonidos.
    /// </summary>
    void Awake()
    {
        // Asegura que solo exista una instancia de AudioManager.
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        globalSoundDictionary = new Dictionary<string, (AudioClip clip, SoundCategory category)>();

        // Recorre cada biblioteca de sonidos (SoundLib) y añade sus sonidos al diccionario global.
        foreach (var library in soundLibsList)
        {
            library.Initialize();
            foreach (var sound in library.soundEntries)
            {
                if (!globalSoundDictionary.ContainsKey(sound.soundName)) globalSoundDictionary.Add(sound.soundName, (sound.clip, sound.category));
            }
        }
    }

	private void Start()
	{
        LoadAllVolumeSettings();
	}

    /// <summary>
    /// Reproduce un sonido específico por su nombre. 
    /// Ideal para música de fondo, sonido ambiente, etc.
    /// </summary>
    /// <param name="soundName">El nombre del sonido que se desea reproducir.</param>
    public void Play(string soundName, float pitch = 1.0f, float spatialBlend = 0.0f, float dopplerLevel = 0.0f, bool loop = true, float volume = 1.0f)
    {
        if (globalSoundDictionary.TryGetValue(soundName, out var entry))
        {
            if (currentMusicName == soundName && musicSource != null && musicSource.isPlaying)
            {
                return;
            }

            if(musicSource != null)
            {
                musicSource.Stop();
                Destroy(musicSource);
            }

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = entry.clip;
            musicSource.outputAudioMixerGroup = GetGroup(entry.category);
            musicSource.volume = volume;
            musicSource.spatialBlend = spatialBlend;
            musicSource.dopplerLevel = dopplerLevel;
            musicSource.pitch = pitch;
            musicSource.loop = loop;

            if (!musicSource.isPlaying) musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in any library.");
        }
    }

    /// <summary>
    /// Reproduce un sonido y hace que siga al GameObject asociado.
    /// </summary>
    /// <param name="soundName">El nombre del sonido que se desea reproducir.</param>
    /// <param name="targetObject">El GameObject al que el sonido debe seguir.</param>
    public void Play3D(string soundName, GameObject targetObject, float spatialBlend = 1.0f, float pitch = 1.0f, float dopplerLevel = 0.0f, bool loop = false, float volume = 1.0f, float minDistance = 6f, bool playIfAlreadyPlaying = false)
    {
        // Busca el AudioClip correspondiente en el diccionario global.
        if (globalSoundDictionary.TryGetValue(soundName, out var entry))
        {
            AudioSource audioSource = targetObject.AddComponent<AudioSource>();

            audioSource.clip = entry.clip;
            audioSource.outputAudioMixerGroup = GetGroup(entry.category);
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.dopplerLevel = dopplerLevel;
            audioSource.pitch = pitch;
            audioSource.minDistance = minDistance;
            audioSource.loop = loop; // Para testeo

            if (playIfAlreadyPlaying)
            {
                audioSource.Play();
            }
            else
            {
                if (!audioSource.isPlaying) audioSource.Play();
            }

        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in any library.");
        }
    }

    /// <summary>
    /// Reproduce un sonido en un punto fijo basado en la posición de un GameObject.
    /// El sonido permanece incluso si el GameObject original se destruye.
    /// Ideal para casos en los que el sonido no debe seguir a un GameObject específico. (ej. explosiones, disparos, etc.)
    /// </summary>
    /// <param name="soundName">El nombre del sonido que se desea reproducir.</param>
    /// <param name="position">La posición en el espacio donde se reproducirá el sonido.</param>
    public void PlayAtPoint(string soundName, Vector3 position, float spatialBlend = 1.0f, float pitch = 1.0f, float dopplerLevel = 0.0f, bool loop = false, float volume = 1.0f)
    {
        // Busca el AudioClip correspondiente en el diccionario global.
        if (globalSoundDictionary.TryGetValue(soundName, out var entry))
        {
            // Crear un GameObject temporal para reproducir el sonido.
            GameObject tempAudioObject = new($"TempAudio_{soundName}");
            tempAudioObject.transform.position = position;

            // Añadir un AudioSource al GameObject temporal.
            AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = entry.clip;
            tempAudioSource.outputAudioMixerGroup = GetGroup(entry.category);
            tempAudioSource.volume = volume;
            tempAudioSource.pitch = pitch;
            tempAudioSource.spatialBlend = spatialBlend;
            tempAudioSource.dopplerLevel = dopplerLevel;
            tempAudioSource.loop = loop; // Para testeo

            tempAudioSource.Play();

            // Destruir el GameObject después de que termine el sonido.
            Destroy(tempAudioObject, entry.clip.length);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found in any library.");
        }
    }
    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
		audioMixer.SetFloat("MasterVolume", AudioMathHelper.ToDecibels(masterVolume));
        SaveMasterVolumeSettings();

        if(masterSlider!=null) masterSlider.value = masterVolume;
	}
	/// <summary>
	/// Ajusta el volumen de la música en el AudioMixer.
	/// </summary>
	/// <param name="volume">El volumen de la música, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
	public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
		audioMixer.SetFloat("MusicVolume", AudioMathHelper.ToDecibels(musicVolume));
		SaveMusicVolumeSettings();
		if (musicSlider != null) musicSlider.value = musicVolume;
	}
	/// <summary>
	/// Ajusta el volumen de los efectos de sonido (SFX) en el AudioMixer.
	/// </summary>
	/// <param name="volume">El volumen de los efectos de sonido, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
	public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
		audioMixer.SetFloat("SFXVolume", AudioMathHelper.ToDecibels(sfxVolume));
		SaveSFXVolumeSettings();
		if (sfxSlider != null) sfxSlider.value = sfxVolume;

	}
	/// <summary>
	/// Ajusta el volumen de los sonidos de la interfaz de usuario (UI) en el AudioMixer.
	/// </summary>
	/// <param name="volume">El volumen de los sonidos de la interfaz de usuario, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
	public void SetUiVolume(float volume)
    {
        uiVolume = volume;
		audioMixer.SetFloat("UIVolume", AudioMathHelper.ToDecibels(uiVolume));
		SaveUIVolumeSettings();
		if (uiSlider != null) uiSlider.value = uiVolume;
	}

	public void LoadAllVolumeSettings() { 
        LoadMasterVolumeSettings();
        LoadMusicVolumeSettings();
        LoadSFXVolumeSettings();
        LoadUIVolumeSettings();
    }

    public void SaveAllVolumeSettings() {
        SaveMasterVolumeSettings();
        SaveMusicVolumeSettings();
        SaveSFXVolumeSettings();
        SaveUIVolumeSettings();
    }

    public void LoadMasterVolumeSettings() {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume"));
	}

	public void LoadMusicVolumeSettings()
	{
		SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
	}

	public void LoadSFXVolumeSettings()
	{
		SetSfxVolume(PlayerPrefs.GetFloat("SFXVolume"));
	}

	public void LoadUIVolumeSettings()
	{
		SetUiVolume(PlayerPrefs.GetFloat("UIVolume"));
	}

	public void SaveMasterVolumeSettings() {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.Save();
    }

	public void SaveMusicVolumeSettings()
	{
		PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
	}

	public void SaveSFXVolumeSettings()
	{
		PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
	}

	public void SaveUIVolumeSettings()
	{
		PlayerPrefs.SetFloat("UIVolume", uiVolume);
        PlayerPrefs.Save();
	}

	private void OnValidate()
	{
        masterVolume = masterVolume;
        musicVolume = musicVolume;
        sfxVolume = sfxVolume;
        uiVolume = uiVolume;

        SaveAllVolumeSettings();
	}

	/// <summary>
	/// Obtiene el grupo de mezcla (`AudioMixerGroup`) correspondiente a la categoría de sonido especificada.
	/// </summary>
	/// <param name="category">La categoría del sonido (Music, SFX, UI).</param>
	/// <returns>El `AudioMixerGroup` asociado a la categoría, o `null` si no se encuentra una coincidencia.</returns>
	private AudioMixerGroup GetGroup(SoundCategory category)
    {
        return category switch
        {
            SoundCategory.Music => musicAudioGroup,
            SoundCategory.SFX => sfxAudioGroup,
            SoundCategory.UI => uiAudioGroup,
            _ => null
        };
    }
}