using UnityEngine;

public class HazardBananaPeel : MonoBehaviour
{
    [SerializeField] private float stunAmount;
    private void OnTriggerEnter(Collider other)
    {
        EnemyMovement em;
        if (other.transform.parent.TryGetComponent(out em))
        {
            em.Stun(stunAmount);
            Destroy(gameObject);
        }
    }
}
