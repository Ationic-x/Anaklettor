using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ChangeSceneController : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeSceneInGame(string sceneName)
    {
        StartCoroutine(DestroyLevelInstance(sceneName));
    }

    private IEnumerator DestroyLevelInstance(string sceneName)
    {        
        while (LevelGenerator.Instance != null)
        {
            LevelGenerator.Instance.DestroyInstance();

            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.1f); // small delay

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        HUDController hudController;

        do
        {
            hudController = FindFirstObjectByType<HUDController>();
            yield return null;
        } while (hudController == null);

        // Load animation
        hudController.Load.LoadAnimationDirection(new Vector2Int(0, 1));
        yield return new WaitForEndOfFrame();


        // Wait Async Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Wait animation end
        yield return new WaitForSecondsRealtime(hudController.Load.GetClipDuration());

        // Return default values and synch all
        asyncLoad.allowSceneActivation = true;

        yield return new WaitForEndOfFrame();

        Time.timeScale = 1f; // Reset time scale to normal
    }


}
