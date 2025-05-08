using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// AudioManager gestiona la reproducción de sonidos en el juego.
/// Se encarga de inicializar un diccionario global de sonidos a partir de múltiples bibliotecas de sonidos (Instancias de SoundLib).
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Range(0, 1)] public float masterVolume;
    [Range(0, 1)] public float musicVolume;
    [Range(0, 1)] public float sfxVolume;
    [Range(0, 1)] public float uiVolume;

    public AudioMixer audioMixer;
    public AudioMixerGroup musicAudioGroup;
    public AudioMixerGroup sfxAudioGroup;
    public AudioMixerGroup uiAudioGroup;

    [Tooltip("List of SoundLibs")]
    public List<SoundLib> soundLibsList;

    private Dictionary<string, (AudioClip clip, SoundCategory category)> globalSoundDictionary;

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
    /// <summary>
    /// Método Update se ejecuta una vez por frame.
    /// En este caso, se utiliza para actualizar dinámicamente los volúmenes de las categorías de sonido
    /// (Music, SFX, UI) en función de los valores configurados en el inspector.
    /// </summary>
    private void Update()
    {
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
        SetUiVolume(uiVolume);
    }

    /// <summary>
    /// Reproduce un sonido específico por su nombre. 
    /// Ideal para música de fondo, sonido ambiente, etc.
    /// </summary>
    /// <param name="soundName">El nombre del sonido que se desea reproducir.</param>
    public void Play(string soundName)
    {
        // Busca el AudioClip correspondiente en el diccionario global.
        if (globalSoundDictionary.TryGetValue(soundName, out var entry))
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = entry.clip;
            audioSource.outputAudioMixerGroup = GetGroup(entry.category);
            audioSource.spatialBlend = 0.0f;
            audioSource.loop = true;

            if (!audioSource.isPlaying) audioSource.Play();
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
    public void Play3D(string soundName, GameObject targetObject)
    {
        // Busca el AudioClip correspondiente en el diccionario global.
        if (globalSoundDictionary.TryGetValue(soundName, out var entry))
        {
            // Verificar si el GameObject ya tiene un AudioSource.
            if (!targetObject.TryGetComponent<AudioSource>(out var audioSource)) audioSource = targetObject.AddComponent<AudioSource>();

            audioSource.clip = entry.clip;
            audioSource.outputAudioMixerGroup = GetGroup(entry.category);
            audioSource.spatialBlend = 1.0f; 
            audioSource.loop = false; // Para testeo

            if (!audioSource.isPlaying) audioSource.Play();
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
    public void PlayAtPoint(string soundName, Vector3 position)
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
            tempAudioSource.spatialBlend = 1.0f;
            tempAudioSource.loop = false; // Para testeo

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
        if (volume == 0)
        {
            audioMixer.SetFloat("MasterVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", (float)(Math.Log10(volume) * 20));
        }
    }
    /// <summary>
    /// Ajusta el volumen de la música en el AudioMixer.
    /// </summary>
    /// <param name="volume">El volumen de la música, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
    public void SetMusicVolume(float volume)
    {
        if (volume == 0) {
            audioMixer.SetFloat("MusicVolume", -80.0f);
        }
        else 
        { 
            audioMixer.SetFloat("MusicVolume", (float)(Math.Log10(volume) * 20)); 
        }
    }
    /// <summary>
    /// Ajusta el volumen de los efectos de sonido (SFX) en el AudioMixer.
    /// </summary>
    /// <param name="volume">El volumen de los efectos de sonido, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
    public void SetSfxVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("SFXVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", (float)(Math.Log10(volume) * 20));
        }
    }
    /// <summary>
    /// Ajusta el volumen de los sonidos de la interfaz de usuario (UI) en el AudioMixer.
    /// </summary>
    /// <param name="volume">El volumen de los sonidos de la interfaz de usuario, un valor entre 0.0 (silencio) y 1.0 (volumen máximo).</param>
    public void SetUiVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("UIVolume", -80.0f);
        }
        else
        {
            audioMixer.SetFloat("UIVolume", (float)(Math.Log10(volume) * 20));
        }
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