using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    public int Cost;
    public int Level;
    public int MaxLevel;

    public void Upgrade()
    {
        Level++;
    }
}
