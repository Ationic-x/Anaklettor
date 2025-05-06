using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    #region Gloabal Attributes
    [SerializeField] string sceneToLoad;

    #endregion

    #region Methods
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    #endregion
}
