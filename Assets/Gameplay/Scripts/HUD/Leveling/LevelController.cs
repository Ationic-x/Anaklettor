using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _optionsUI;

    private readonly string[] _optionsText = new string[]
    {
        "Incrementa el daño",
        "Incrementa la vida",
        "Incrementa la velocidad de ataque",
        "Incrementa la velocidad de movimiento",
        "Incrementa la velocidad de defensa",
        "Reduce el retardo entre ataques",
        "Reduce el retardo enntre defensa",
    };

    private void OnEnable()
    {
        HashSet<int> opcionesElegidas = new HashSet<int>();
        foreach (var option in _optionsUI)
        {
            int ot;
            do
            {
                ot = Random.Range(0, _optionsText.Length);
            } while (opcionesElegidas.Contains(ot));

            opcionesElegidas.Add(ot);

            option.GetComponentInChildren<TextMeshProUGUI>().text = _optionsText[ot];
        }
    }
}
