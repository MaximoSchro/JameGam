using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputAction PauseAction;
    private void OnEnable()
    {
        PauseAction.Enable();
    }
    private void OnDisable()
    {
        PauseAction.Disable();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        PauseAction.performed += context => ChangeMouseLock();
    }
    
    private void ChangeMouseLock()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
