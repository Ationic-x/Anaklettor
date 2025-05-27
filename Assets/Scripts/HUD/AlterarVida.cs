using UnityEngine;
using UnityEngine.UI;

public class AlterarVida : MonoBehaviour
{
    Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
    }

    public void UpdateHP(float f)
    {
        image.fillAmount = f;
    }
}
