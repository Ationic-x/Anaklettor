using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectManager : MonoBehaviour
{
    public IJob.Jobs selectedCharacter;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SelectCharacter(Job character)
    {
        selectedCharacter = character.Class;
        SceneManager.LoadScene("PlayerTest");
    }
}
