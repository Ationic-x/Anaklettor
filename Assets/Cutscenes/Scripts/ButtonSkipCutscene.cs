using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSkipCutscene : MonoBehaviour
{
    #region Global Variables
    private GameObject buttonSkip;

    #endregion

    #region Unity Methods
    private void Start()
    {
        buttonSkip = GetComponent<GameObject>();
    }//EndOf Unity method Start

    #endregion

    #region Own Scripts
    public void SkipCutscene()
    {
        SceneManager.LoadScene("Start");
    }//EndOf method SkipCutscene

    #endregion
}
