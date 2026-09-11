using UnityEngine;
using UnityEngine.Splines;

public class EnemyPath : MonoBehaviour
{
    public static EnemyPath Instance { get; private set; }
    public SplineContainer sc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (!TryGetComponent<SplineContainer>(out sc))
        {
            sc = new SplineContainer();
        }
    }
}
