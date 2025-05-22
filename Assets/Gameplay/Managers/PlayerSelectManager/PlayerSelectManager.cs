using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectManager : MonoBehaviour
{
    public Jobs selectedCharacter;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SelectCharacter(Jobs character)
    {
        selectedCharacter = character;
        SceneManager.LoadScene("PlayerTest");
    }
}
