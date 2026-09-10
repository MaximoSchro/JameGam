using UnityEngine;

public class ShovelHead : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && !playerController.CheckAnimationName("ShovelSwing")) return;
        int currentCharge = playerController.GetChargeState();
        Vector3 cameraForward = Camera.main.transform.forward;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.AddForceAtPosition(cameraForward * 100 * Mathf.Pow(5, currentCharge), other.ClosestPoint(transform.position));
    }
}
