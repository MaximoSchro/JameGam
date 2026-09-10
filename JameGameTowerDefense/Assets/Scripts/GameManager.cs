using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public InputAction PauseAction;
    public InputAction AddCurrency;
 
    [SerializeField] private TMP_Text currencyTracker;
    [SerializeField] private GameObject PauseMenu;

    private bool gamePaused = false;

    private int currency;
    public int Currency
    {
        get { return currency; }
        set
        {
            currency = value;
            currencyTracker.text = $"{currency}";
        }
    }

    private void OnEnable()
    {
        Time.timeScale = 1;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        PauseAction.Enable();
        AddCurrency.Enable();
    }
    private void OnDisable()
    {
        PauseAction.Disable();
        AddCurrency.Disable();
        Time.timeScale = 1;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        PauseAction.performed += context => PauseUnpause();
        AddCurrency.performed += context => UpdateCurrency(10);
        SetCurrency(0);
        PauseMenu.SetActive(false);
    }
    public void PauseUnpause()
    {
        gamePaused = !gamePaused;
        PauseMenu.SetActive(gamePaused);
        Cursor.lockState = gamePaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = gamePaused ? 0 : 1;
    }
    public void UpdateCurrency(int delta)
    {
        Currency += delta;
    }
    private void SetCurrency(int currency)
    {
        Currency = currency;
    }
    public bool PurchaseItem(int cost)
    {
        if(Currency >= cost)
        {
            Currency -= cost;
            return true;
        }
        return false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
