using UnityEngine;

public class BananaTree : TowerBase
{
    [SerializeField] float spawnTime = 10f;
    [SerializeField] float upgradeAmount = 2f;
    
    [SerializeField] Transform BananaPrefab;

    private float _time = 0;

    private void Update()
    {
        _time += Time.deltaTime;
        
        if (_time > (spawnTime - upgradeAmount * Level))
        {
            SpawnBanana();
        }
    }

    private void SpawnBanana()
    {
        Instantiate(BananaPrefab, transform.GetChild(0).GetChild(Random.Range(0, 3)).transform.position,  Quaternion.identity);
        _time = 0;
    }
}
