using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
    public void ActivatePhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }//EndOf method ActivatePhysics

    public void DeactivatePhysics()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }//EndOf method DeactivatePhysics
}
