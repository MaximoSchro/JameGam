using Unity.Mathematics;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ShovelHead : MonoBehaviour
{
    public static Action TriggeredThirdCharge;
    public static Action SwingAnimFinished;

    [SerializeField] private PlayerController playerController;

    [SerializeField] private float boxHalfWidth;
    private bool waitingForThird = false;

    private void OnEnable()
    {
        SwingAnimFinished += TriggerThird;
        TriggeredThirdCharge += PrepareThird;
    }
    private void OnDisable()
    {
        SwingAnimFinished -= TriggerThird;
        TriggeredThirdCharge -= PrepareThird;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" && !playerController.CheckAnimationName("ShovelSwing")) return;
        if (waitingForThird) return;
        Vector3 cameraForward = Camera.main.transform.forward;
        int currentCharge = playerController.GetChargeState();
        Rigidbody rb;
        switch (currentCharge)
        {
            case 1:
                if (other.TryGetComponent<EnemyMovement>(out EnemyMovement em))
                {
                    em.Stun(playerController.GetStunTime());
                }
                break;
            case 2:
                rb = other.GetComponent<Rigidbody>();
                rb.AddForceAtPosition(cameraForward * 100 * Mathf.Pow(5, currentCharge), other.ClosestPoint(transform.position));
                if (rb.gameObject.TryGetComponent<EnemyHealth>(out EnemyHealth health))
                {
                    health.TimedDeath(5);
                }
                if (rb.TryGetComponent<EnemyMovement>(out EnemyMovement movement))
                {
                    Destroy(movement);
                }
                break;
        }
    }
    private void PrepareThird()
    {
        waitingForThird = true;
    }
    private void TriggerThird()
    {
        if (!waitingForThird) return;
        Vector3 cameraForward = Camera.main.transform.forward;
        Rigidbody rb;
        RaycastHit[] hits = Physics.BoxCastAll(Camera.main.transform.position + cameraForward * boxHalfWidth/2, Vector3.one * boxHalfWidth, cameraForward, Camera.main.transform.rotation);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag != "Enemy") continue;
            rb = hit.transform.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(cameraForward * 100 * Mathf.Pow(5, 3), this.transform.position);
            if (rb.gameObject.TryGetComponent<EnemyHealth>(out EnemyHealth health))
            {
                health.TimedDeath(5);
            }
            if (rb.TryGetComponent<EnemyMovement>(out EnemyMovement movement))
            {
                rb.freezeRotation = false;
                Destroy(movement);
            }
        }
        waitingForThird = false;
    }
}
