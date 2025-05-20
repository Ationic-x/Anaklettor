using UnityEngine;

public class IAEventsBridgeAnaklettor : MonoBehaviour
{

    AnaklettorIAController anaklettorIAController;

    private void Start()
    {
        anaklettorIAController = transform.GetComponentInChildren<AnaklettorIAController>();
    }

    public void TrhowSpell()
    {
        anaklettorIAController.TrhowSpell();
    }
}
