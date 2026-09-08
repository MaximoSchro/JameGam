using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public InputAction BuildModeAction;
    public InputAction PlaceBuildAction;

    public bool InBuildMode = false;
    public int currentTowerIndex = 0;

    [SerializeField] private GameObject BuildUI;
    [SerializeField] private GameObject[] TowerOutlines;
    [SerializeField] private GameObject[] Towers;

    private GameObject mainCamera;
    private GameObject OutlineObject;
    private int floorMask;

    private void OnEnable()
    {
        BuildModeAction.Enable();
        PlaceBuildAction.Enable();
    }
    private void OnDisable()
    {
        BuildModeAction.Disable();
        PlaceBuildAction.Disable();
    }
    private void Start()
    {
        BuildModeAction.performed += context => SwitchBuildMode();
        PlaceBuildAction.performed += context => PlaceBuild();
        mainCamera = Camera.main.gameObject;
        floorMask = LayerMask.GetMask("Floor");

        SetOutlineObject(TowerOutlines[currentTowerIndex]);
        OutlineObject.SetActive(InBuildMode);
        BuildUI.SetActive(InBuildMode);
    }
    private void SetOutlineObject(GameObject newOutlineObject)
    {
        if (OutlineObject != null) Destroy(OutlineObject);
        OutlineObject = Instantiate(TowerOutlines[currentTowerIndex]);
    }
    private void SwitchBuildMode()
    {
        InBuildMode = !InBuildMode;
        BuildUI.SetActive(InBuildMode);
        OutlineObject.SetActive(InBuildMode);
        OutlineObject.transform.position = RaycastToFloor();
    }
    private void PlaceBuild()
    {
        if (!InBuildMode) return;
        Instantiate(Towers[currentTowerIndex], RaycastToFloor(), Quaternion.identity);
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
            Debug.Log(hit.transform.position);
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
