using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMusicAudioTrigger : MonoBehaviour
{
    private string sceneName;

    private string[] soundsNames = {
       SoundsNames.ExplorationMusic,
       SoundsNames.AnakletorFightMusic,
       SoundsNames.MainMenuMusic1,
       SoundsNames.AnakletorCutscene,
   };

    private string currentMusic;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        sceneName = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        PlayMusicForScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneName = scene.name;
        PlayMusicForScene(sceneName);
    }

    private void PlayMusicForScene(string sceneName)
    {
        string musicToPlay = sceneName switch
        {
            "Menus" => soundsNames[2],
            "End" => soundsNames[1],
            "Start_Cutscene" => soundsNames[3],
            _ => soundsNames[0],
        };
        if (currentMusic == musicToPlay)
            return;

        currentMusic = musicToPlay;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(musicToPlay, loop: true, volume: 0.22f);
        }
    }
}
