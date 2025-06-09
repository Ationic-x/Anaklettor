using System.Collections;
using UnityEngine;

public class PauseGUIController : MonoBehaviour
{

    private bool isPaused = false;
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // This should use input map fix in future
    private void Update()
    {
        if (HUDController.isBusy)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }


    private void PauseGame()
    {
        Time.timeScale = 0;
        _animator.SetTrigger("In");
        isPaused = true;
    }

    public void ResumeGame()
    {
        _animator.SetTrigger("Out");
        StartCoroutine(ResumeAfterSeconds(0.5f));
        isPaused = false;
    }

    IEnumerator ResumeAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1;
    }

}
