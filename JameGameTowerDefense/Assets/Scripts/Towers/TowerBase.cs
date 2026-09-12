using UnityEngine;
using System.Collections.Generic;

public abstract class TowerBase : MonoBehaviour
{
    public int Cost;
    public int Level;
    public int MaxLevel;

    private List<Material> baseMaterials = new List<Material>();
    private MeshRenderer[] meshRenderers;
    public void Initialize()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mr in meshRenderers)
        {
            baseMaterials.Add(mr.sharedMaterial);
        }
        Cost += Cost;
    }
    public void Upgrade()
    {
        if (Cost > GameManager.Instance.Currency) return;
        GameManager.Instance.PurchaseItem(Cost);
        Level++;
        Cost += Cost;
    }
    public void ResetMaterial()
    {
        for(int i = 0; i < baseMaterials.Count; i++)
        {
            meshRenderers[i].sharedMaterial = baseMaterials[i];
        }
    }
    public void SetMaterial(Material mat)
    {
        foreach (MeshRenderer mr in meshRenderers)
        {
            mr.sharedMaterial = mat;
        }
    }
}
