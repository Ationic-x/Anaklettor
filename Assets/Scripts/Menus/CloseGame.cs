using UnityEngine;

public class CloseGame : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }//EndOf method QuitGame
}
