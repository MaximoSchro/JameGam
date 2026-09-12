using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static bool InWave = false;
    public InputAction PauseAction;
    public InputAction AddCurrency;
    public InputAction StartWaveAction;
 
    [SerializeField] private TMP_Text currencyTracker;
    [SerializeField] private GameObject PauseMenu;

    [SerializeField] private int StartingCurrency;

    [SerializeField] private WaveData[] Waves;
    private int waveIndex = 0;

    private bool gamePaused = false;

    private int currency;
    private List<GameObject> enemyList = new List<GameObject>();
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
        SetCurrency(StartingCurrency);
        PauseMenu.SetActive(false);
    }
    private void Update()
    {
        if(InWave && enemyList.Count <= 0)
        {
            InWave = false;
        }
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
    public void StartWave()
    {
        if (InWave) return;
        InWave = true;
        StartCoroutine(HandleWave(Waves[waveIndex]));
        waveIndex++;
    }
    private IEnumerator HandleWave(WaveData wave)
    {
        int index = 0;
        while(index < wave.EnemiesToSpawn.Length)
        {
            GameObject temp = Instantiate(wave.EnemiesToSpawn[index]);
            enemyList.Add(temp);
            index++;
            yield return new WaitForSeconds(0.5f);
        }
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
