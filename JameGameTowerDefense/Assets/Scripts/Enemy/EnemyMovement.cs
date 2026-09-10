using UnityEngine;
using UnityEngine.Splines;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    
    private SplineContainer spline;
    private float dist;
    
    void Start()
    {
        //will change this to a singleton spline if i care later
        spline = GameObject.Find("Spline").GetComponent<SplineContainer>();
        dist = 0;
    }

    void Update()
    {
        dist += Time.deltaTime * speed;
        SplineFunctions.SplineEvaluate(spline, dist, out Vector3 pos, out float t);
        
        transform.position = pos;
        if (t >= 1) Destroy(gameObject); //do whatever losing thing is supposed to happen when they get to the end
    }
}
