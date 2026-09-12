using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public InputAction BuildModeAction;
    public InputAction PlaceBuildAction;
    public InputAction ChangeSelectedBuildAction;
    public InputAction ScrollChangeSelectedBuildAction;

    public bool InBuildMode = false;
    public int currentTowerIndex = 0;

    [SerializeField] private GameObject BuildUI;
    [SerializeField] private GameObject[] TowerOutlines;
    [SerializeField] private GameObject[] Towers;

    private GameObject mainCamera;
    private GameObject OutlineObject;
    private int floorMask;

    private Animator currentHotbar;

    private void OnEnable()
    {
        BuildModeAction.Enable();
        PlaceBuildAction.Enable();
        ChangeSelectedBuildAction.Enable();
        ScrollChangeSelectedBuildAction.Enable();
    }
    private void OnDisable()
    {
        BuildModeAction.Disable();
        PlaceBuildAction.Disable();
        ChangeSelectedBuildAction.Disable();
        ScrollChangeSelectedBuildAction.Disable();
    }
    private void Start()
    {
        BuildModeAction.performed += context => SwitchBuildMode();
        PlaceBuildAction.performed += context => PlaceBuild();
        ChangeSelectedBuildAction.performed += context => KeyChangedHotbar(context.control.name);
        ScrollChangeSelectedBuildAction.performed += context => ScrollChangedHotbar(context.ReadValue<Vector2>().y);

        mainCamera = Camera.main.gameObject;
        floorMask = LayerMask.GetMask("Floor");

        SetOutlineObject(currentTowerIndex);
        InBuildMode = false;
        OutlineObject.SetActive(InBuildMode);
        BuildUI.SetActive(InBuildMode);
        
    }
    private void SetOutlineObject(int target)
    {
        if (OutlineObject != null) Destroy(OutlineObject);
        OutlineObject = Instantiate(TowerOutlines[target]);
    }
    private void SwitchBuildMode()
    {
        InBuildMode = !InBuildMode;
        BuildUI.SetActive(InBuildMode);
        OutlineObject.SetActive(InBuildMode);
        OutlineObject.transform.position = RaycastToFloor();
        UpdateHotbar(currentTowerIndex + 1);
        PlayerController.SetSwingAction?.Invoke(!InBuildMode);
    }
    private void KeyChangedHotbar(string key)
    {
        if (!InBuildMode) return;
        if(int.TryParse(key, out int num))
        {
            ChangeHotBarTarget(num-1);
        }
    }
    private void ScrollChangedHotbar(float delta)
    {
        if (!InBuildMode) return;
        if (delta > 0)
        {
            currentTowerIndex++;
            if(currentTowerIndex >= TowerOutlines.Length)
            {
                currentTowerIndex = 0;
            }
        }
        else
        {
            currentTowerIndex--;
            if(currentTowerIndex < 0)
            {
                currentTowerIndex = TowerOutlines.Length - 1;
            }
        }
        ChangeHotBarTarget(currentTowerIndex);
    }
    private void ChangeHotBarTarget(int target)
    {
        if (target < 0 || target >= TowerOutlines.Length) return;
        currentTowerIndex = target;
        SetOutlineObject(currentTowerIndex);
        UpdateHotbar(currentTowerIndex + 1);
    }
    private void UpdateHotbar(int newTarget)
    {
        if(currentHotbar != null)
        {
            currentHotbar.SetTrigger("PlayDeselect");
        }
        GameObject newHotBar = GameObject.Find($"HotbarItem{newTarget}");
        if(newHotBar != null)
        {
            Animator animator = newHotBar.GetComponentInChildren<Animator>();
            animator.SetTrigger("PlaySelect");
            currentHotbar = animator;
        }
    }
    private void PlaceBuild()
    {
        if (!InBuildMode) return;
        GameObject temp = Towers[currentTowerIndex];
        if(temp.TryGetComponent<TowerBase>(out TowerBase tower))
        {
            if(GameManager.Instance.PurchaseItem(tower.Cost)) 
                Instantiate(temp, RaycastToFloor(), Quaternion.identity);
        }
    }

    private void Update()
    {
        if (!InBuildMode) return;
        OutlineObject.transform.position = RaycastToFloor();
        
    }
    private Vector3 RaycastToFloor()
    {
        mainCamera = Camera.main.gameObject;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, floorMask))
        {
            return hit.point;
        }

#if UNITY_EDITOR
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 1000f, floorMask))
        {
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red);
        }
        else
        {
            Vector3 endPoint = mainCamera.transform.position + (mainCamera.transform.TransformDirection(Vector3.forward) * 10000);
            Debug.DrawLine(mainCamera.transform.position, hit.point, Color.red);
        }
#endif
        return Vector3.one * 10000f;
    }
}
