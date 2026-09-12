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
    [SerializeField] private Material CanAffordMaterial;
    [SerializeField] private Material CannotAffortMaterial;

    private GameObject mainCamera;
    private GameObject OutlineObject;
    private int floorMask;
    private int towerMask;
    private LayerMask combinedMask;
    private Vector3 noCast = Vector3.one * 10000f;

    private Animator currentHotbar;

    private GameObject currentlyLookedAtTower;
    private GameObject CurrentLookedAtTower
    {
        get {  return currentlyLookedAtTower; }
        set
        {
            if(currentlyLookedAtTower != null)
             currentlyLookedAtTower.GetComponent<TowerBase>().ResetMaterial();
            currentlyLookedAtTower = value;
        }
    }

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
        towerMask = LayerMask.GetMask("Tower");
        combinedMask = LayerMask.GetMask("Tower", "Floor");

        SetOutlineObject(currentTowerIndex);
        InBuildMode = false;
        OutlineObject.SetActive(InBuildMode);
        BuildUI.SetActive(InBuildMode);
        
    }
    private void SetOutlineObject(int target)
    {
        if (OutlineObject != null) Destroy(OutlineObject);
        OutlineObject = Instantiate(TowerOutlines[target], RaycastToFloor(), Quaternion.identity);
        Quaternion rotation = new Quaternion(OutlineObject.transform.rotation.x, mainCamera.transform.rotation.y,
                    OutlineObject.transform.rotation.z, OutlineObject.transform.rotation.w);
        OutlineObject.transform.rotation = rotation;
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
        if(CurrentLookedAtTower != null)
        {
            CurrentLookedAtTower.GetComponent<TowerBase>().Upgrade();
        }
        else
        {
            GameObject temp = Towers[currentTowerIndex];
            if (temp.TryGetComponent<TowerBase>(out TowerBase tower))
            {
                if (GameManager.Instance.PurchaseItem(tower.Cost))
                {
                    Quaternion rotation = new Quaternion(temp.transform.rotation.x, mainCamera.transform.rotation.y,
                        temp.transform.rotation.z, temp.transform.rotation.w);
                    GameObject building = Instantiate(temp, RaycastToFloor(), rotation);
                    building.GetComponent<TowerBase>().Initialize();
                }
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (!InBuildMode) return;
        if (InBuildMode && GameManager.InWave)
        {
            SwitchBuildMode();
            return;
        }
        GameObject obj;
        if (CheckCastToFloor())
        {
            OutlineObject.SetActive(true);
            CurrentLookedAtTower = null;
            Vector3 pos = RaycastToFloor();
            OutlineObject.transform.position = pos;
            OutlineObject.transform.rotation = new Quaternion(OutlineObject.transform.rotation.x, mainCamera.transform.rotation.y,
                        OutlineObject.transform.rotation.z, OutlineObject.transform.rotation.w);
            MeshRenderer[] mrs = OutlineObject.GetComponentsInChildren<MeshRenderer>();
            if (Towers[currentTowerIndex].GetComponent<TowerBase>().Cost <= GameManager.Instance.Currency)
            {
               foreach (MeshRenderer mr in mrs)
               {
                    mr.sharedMaterial = CanAffordMaterial;
               }
            }
            else
            {
                foreach (MeshRenderer mr in mrs)
                {
                    mr.sharedMaterial = CannotAffortMaterial;
                }
            }
        }
        else if(RaycastToTower(out obj))
        {
            OutlineObject.SetActive(false);
            if (obj != CurrentLookedAtTower)
            {
                CurrentLookedAtTower = obj;
            }
            TowerBase tower = currentlyLookedAtTower.GetComponent<TowerBase>();
            if(tower.Cost <= GameManager.Instance.Currency)
            {
                tower.SetMaterial(CanAffordMaterial);
            }
            else
            {
                tower.SetMaterial(CannotAffortMaterial);
            }
        }

    }
    private Vector3 RaycastToFloor()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, floorMask))
        {
            return hit.point;
        }
        return noCast;
    }
    private bool RaycastToTower(out GameObject tower)
    {
        tower = null;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, towerMask))
        {
            tower = hit.transform.gameObject;
            return true;
        }
        return false;
    }
    private bool CheckCastToFloor()
    {
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 1000f, combinedMask))
        {
            if(hit.transform.gameObject.layer == 6)
                return true;
            else return false;
        }
        return false;
    }
}
