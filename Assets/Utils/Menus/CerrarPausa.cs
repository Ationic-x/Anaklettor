using UnityEngine;
using UnityEngine.SceneManagement;

public class CerrarPausa : MonoBehaviour
{
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
