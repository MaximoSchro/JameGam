using System;
using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public abstract class PlayerUpgrades
{
    public string Title;
    public int Cost;
    public abstract bool Upgrade();
}
[Serializable]
public class SwingUpgrade : PlayerUpgrades
{
    public int TierToUnlock = 0;
    public override bool Upgrade()
    {
        if (GameManager.Instance.Currency < Cost) return false;
        GameManager.Instance.Currency -= Cost;
        PlayerController.UpgradeSwingTier?.Invoke(TierToUnlock);
        return true;
    }
}
[Serializable]
public class StunUpgrade : PlayerUpgrades
{
    public float StunTimeIncrease = 0;
    public override bool Upgrade()
    {
        if (GameManager.Instance.Currency < Cost) return false;
        GameManager.Instance.Currency -= Cost;
        PlayerController.IncreaseStunTime?.Invoke(StunTimeIncrease);
        return true;
    }
}

public class ShopInteractable : MonoBehaviour
{
    public InputAction EndShopAction;
    [SerializeField] private GameObject ShopCamera;
    [SerializeField] private GameObject ShopUI;
    [SerializeField] private TMP_Text SwingButtonText;
    [SerializeField] private TMP_Text StunButtonText;
    [SerializeReference, SubclassSelector] private SwingUpgrade[] SwingUpgrades;
    [SerializeReference, SubclassSelector] private StunUpgrade[] StunUpgrades;
    
    private bool inShop = false;
    private int swingUpgradeIndex = 0;
    private int stunUpgradeIndex = 0; 

    private void OnEnable()
    {
        EndShopAction.Enable();
    }
    private void OnDisable()
    {
        EndShopAction.Disable();
    }
    private void Start()
    {
        EndShopAction.performed += context => EndShop();

        SwingButtonText.text = $"{SwingUpgrades[0].Title}\nCost:{SwingUpgrades[0].Cost}";
        StunButtonText.text = $"{StunUpgrades[0].Title}\nCost:{StunUpgrades[0].Cost}";

        ShopCamera.SetActive(false);
        ShopUI.SetActive(false);
        inShop = false;
    }
    public void StartShop()
    {
        StartCoroutine(TimeBeforeExit());
        Cursor.lockState = CursorLockMode.Confined;
        ShopCamera.SetActive(true);
        ShopUI.SetActive(true);
    }
    public void EndShop()
    {
        if (!inShop) return;
        Cursor.lockState = CursorLockMode.Locked;
        ShopCamera.SetActive(false);
        ShopUI.SetActive(false);
        inShop = false;
    }
    public void UpgradeSwing()
    {
        if (SwingUpgrades[swingUpgradeIndex].Upgrade())
        {
            swingUpgradeIndex++;
            if(swingUpgradeIndex < SwingUpgrades.Length)
                SwingButtonText.text = $"{SwingUpgrades[swingUpgradeIndex].Title}\nCost:{SwingUpgrades[swingUpgradeIndex].Cost}";
            else
            {
                SwingButtonText.text = "Upgrades Complete";
                SwingButtonText.transform.parent.GetComponent<Button>().interactable = false;
            }
        }
    }
    public void UpgradeStun()
    {
        if (StunUpgrades[stunUpgradeIndex].Upgrade())
        {
            stunUpgradeIndex++;
            if(stunUpgradeIndex < StunUpgrades.Length)
                StunButtonText.text = $"{StunUpgrades[stunUpgradeIndex].Title}\nCost:{StunUpgrades[stunUpgradeIndex].Cost}";
            else
            {
                StunButtonText.text = "Upgrades Complete";
                StunButtonText.transform.parent.GetComponent<Button>().interactable = false;
            }
        }
    }
    private IEnumerator TimeBeforeExit()
    {
        yield return new WaitForSeconds(1f);
        inShop = true;
    }
}
