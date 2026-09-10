using Unity.Mathematics;
using UnityEngine;

public class ShovelHead : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private float boxHalfWidth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && !playerController.CheckAnimationName("ShovelSwing")) return;
        Vector3 cameraForward = Camera.main.transform.forward;
        int currentCharge = playerController.GetChargeState();
        Rigidbody rb;
        switch (currentCharge)
        {
            case 1:
                //do stun
                break;
            case 2:
                    rb = other.GetComponent<Rigidbody>();
                    rb.AddForceAtPosition(cameraForward * 100 * Mathf.Pow(5, currentCharge), other.ClosestPoint(transform.position));
                break;
            case 3:
                RaycastHit[] hits = Physics.BoxCastAll(this.transform.position + this.transform.forward * 5, Vector3.one * boxHalfWidth, this.transform.forward);
                foreach(RaycastHit hit in hits)
                {
                    if(hit.transform.gameObject.tag != "Enemy") continue;
                    rb = hit.transform.GetComponent<Rigidbody>();
                    rb.AddForceAtPosition(cameraForward * 100 * Mathf.Pow(5, currentCharge), other.ClosestPoint(transform.position));
                }
                break;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(this.transform.position + Camera.main.transform.forward * 2 +Vector3.up*2, Vector3.one * boxHalfWidth);
    }
}
