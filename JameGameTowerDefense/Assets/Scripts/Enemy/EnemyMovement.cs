using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    
    [SerializeField] float slowPerStack = 0.1f;
    [SerializeField] private float maxSlowDown = 0.3f;
    
    private SplineContainer _spline;
    private float _dist;
    private bool _isStunned = false;
    
    private int _slowStacks = 0;
    
    void Start()
    {
        //will change this to a singleton spline if i care later
        _spline = GameObject.Find("Spline").GetComponent<SplineContainer>();
        _dist = 0;
    }

    void Update()
    {
        //clean up this line if it upsets anyone
        _dist += !_isStunned ? Time.deltaTime * speed * Mathf.Clamp((1f - slowPerStack * _slowStacks), maxSlowDown, 1f) : 0f;
        SplineFunctions.SplineEvaluate(_spline, _dist, out Vector3 pos, out float t);
        
        transform.position = pos;
        if (t >= 1) Destroy(gameObject); //do whatever losing thing is supposed to happen when they get to the end
    }

    void Stun(float time)
    {
        StartCoroutine("ApplyStun", time);
    }

    IEnumerator ApplyStun(float time)
    {
        _isStunned = true;
        yield return new WaitForSeconds(time);
        _isStunned = false;
    }

    void AddSlowStacks(int amount = 1)
    {
        _slowStacks += amount;
    }
    
    void RemoveSlowStacks(int amount = 1)
    {
        _slowStacks -= amount;
        _slowStacks = _slowStacks < 0 ? 0 : _slowStacks;
    }
    
    void ClearSlowStacks()
    {
        _slowStacks = 0;
    }
}
