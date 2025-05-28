using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneController: MonoBehaviour
{
    #region Methods
    public void ChangeScene(string SeneName)
    {
        SceneManager.LoadScene(SeneName);
    }

    #endregion
}
