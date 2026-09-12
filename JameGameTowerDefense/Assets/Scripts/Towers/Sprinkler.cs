using UnityEngine;

public class Sprinkler : TowerBase
{
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float upgradeAmount = 5f;
    
    private void Update()
    {
        transform.Rotate(Vector3.up, (rotationSpeed + upgradeAmount) * Time.deltaTime);
    }
}
