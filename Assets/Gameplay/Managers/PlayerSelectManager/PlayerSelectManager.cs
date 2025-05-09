using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectManager : MonoBehaviour
{
    public string SelectedCharacter;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SelectCharacter(string characterName)
    {
        SelectedCharacter = characterName;
        Debug.Log("Selected Character: " + SelectedCharacter);

        SceneManager.LoadScene("PlayerTest");
    }
}
