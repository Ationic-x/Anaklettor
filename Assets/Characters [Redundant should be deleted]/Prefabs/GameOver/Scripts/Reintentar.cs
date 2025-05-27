using UnityEngine;
using UnityEngine.SceneManagement;

public class Reintentar : MonoBehaviour
{

    public void ReintentarJuego()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
