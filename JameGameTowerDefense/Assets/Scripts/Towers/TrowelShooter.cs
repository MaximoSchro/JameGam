using Unity.VisualScripting;
using UnityEngine;

public class TrowelShooter : TowerBase
{
    [SerializeField] float spawnTime = 0.5f;
    [SerializeField] float launchSpeed = 5f;
    
    [SerializeField] GameObject trowel1Prefab;
    [SerializeField] GameObject trowel2Prefab;
    [SerializeField] GameObject trowel3Prefab;
    
    private float _time = 0;

    private GameObject target = null;

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        if (_time < spawnTime) return;
        if (target == null) return;

        _time = 0;
        GameObject trowel;
        
        if(Level == 1)
            trowel = Instantiate(trowel1Prefab, transform.position, Quaternion.identity);
        else if(Level == 2)
            trowel = Instantiate(trowel2Prefab, transform.position, Quaternion.identity);
        else
            trowel = Instantiate(trowel3Prefab, transform.position, Quaternion.identity);

        trowel.transform.LookAt(target.transform);
        trowel.GetComponent<Rigidbody>().linearVelocity = (target.transform.position - transform.position) * launchSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!(other.tag == "Enemy")) return;
        
        Debug.Log(transform.forward);
        Debug.Log(other.gameObject.transform.forward);
        
        float temp = Vector3.Dot(transform.forward, (other.gameObject.transform.position - this.transform.position).normalized);
        if (temp < 0.7f) return;
        
        target = other.gameObject;
    }
}
