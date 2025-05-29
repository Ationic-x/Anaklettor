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
        Time.timeScale = 1;
        _animator.SetTrigger("Out");
        isPaused = false;
    }
}
