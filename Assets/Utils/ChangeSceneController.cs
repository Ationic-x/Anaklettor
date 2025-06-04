using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeSceneController: MonoBehaviour
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

		LevelGenerator levelGO;
		do
		{
			levelGO = FindFirstObjectByType<LevelGenerator>();
			yield return null;
		} while (levelGO == null);
		levelGO.DestroyInstance();
		
		while (LevelGenerator.Instance != null)
		{
			yield return null;
		}
		yield return new WaitForSecondsRealtime(0.5f); // small delay

		Time.timeScale = 1f; // Reset time scale to normal
		SceneManager.LoadScene(sceneName);
	}
}
