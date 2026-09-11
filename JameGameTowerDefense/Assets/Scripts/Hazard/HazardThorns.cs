using UnityEngine;

public class HazardThorns : MonoBehaviour
{
    [SerializeField] private int stackAmount = 1;
    private void OnTriggerEnter(Collider other)
    {
        EnemyMovement em;
        if (other.transform.parent.TryGetComponent(out em))
        {
            em.AddSlowStacks(stackAmount);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyMovement em;
        if (other.transform.parent.TryGetComponent(out em))
        {
            em.RemoveSlowStacks(stackAmount);
        }
    }
}
