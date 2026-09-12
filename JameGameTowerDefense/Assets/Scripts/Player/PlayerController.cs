using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.HardwareProfiles;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static Action<bool> SetSwingAction;
    public static Action<int> UpgradeSwingTier;
    public static Action<float> IncreaseStunTime;

    public InputAction MovementAction;
    public InputAction JumpAction;
    public InputAction SwingAction;
    public InputAction InteractAction;

    public bool CanChargeTierTwo;
    public bool CanChargeTierThree;

    [SerializeField] private float speed = 10;
    [SerializeField] private float acceleration = 20;
    [SerializeField] private float jumpPower = 10;

    [SerializeField] private GameObject ShovelObject;
    [SerializeField] private Animator ShovelAnimator;
    [SerializeField] private float ChargeTime;
    [SerializeField] private float ChargeMagnitude;
    [SerializeField] private float StunTime;

    [SerializeField] private GameObject InteractText;
    [SerializeField] private GameObject ShopCamera;

    private CharacterController controller;

    private Vector3 absoluteMovementInput = Vector3.zero;
    private Vector3 relativeMovementInput = Vector3.zero;
    private Vector3 desiredHorizontalMovement = Vector3.zero;
    private float currentVerticalVelocity = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredFinalVelocity = Vector3.zero;

    private bool jumpValue;

    private int shovelChargeState;
    private bool holdingCharge;
    private Coroutine chargeCoroutine;

    private int interactableLayer;
    private GameObject mainCamera;
    private GameObject currentRaycastTarget;


    private void OnEnable()
    {
        MovementAction.Enable();
        JumpAction.Enable();
        SwingAction.Enable();
        InteractAction.Enable();

        SetSwingAction += SetSwing;
        UpgradeSwingTier += SwingUpgrade;
        IncreaseStunTime += StunIncrease;
    }
    private void OnDisable()
    {
        MovementAction.Disable();
        JumpAction.Disable();
        SwingAction.Disable();
        InteractAction.Disable();

        SetSwingAction -= SetSwing;
        UpgradeSwingTier -= SwingUpgrade;
        IncreaseStunTime -= StunIncrease;
    }
    private void Start()
    {
        MovementAction.performed += context => OnMove(context.ReadValue<Vector2>().normalized);
        MovementAction.canceled += context => OnMove(context.ReadValue<Vector2>().normalized);
        JumpAction.performed += context => OnJump(context.ReadValue<float>());
        InteractAction.performed += context => HandleInteraction();

        SwingAction.performed += context => StartSwing();
        SwingAction.canceled += context => EndSwing();

        controller = GetComponent<CharacterController>();
        interactableLayer = LayerMask.GetMask("Interactable");
        mainCamera = Camera.main.gameObject;
    }
    private void Update()
    {
        if (!ShopCamera.activeInHierarchy)
        {
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 5f, interactableLayer))
            {
                currentRaycastTarget = hit.transform.gameObject;
                InteractText.SetActive(true);
            }
            else
            {
                currentRaycastTarget = null;
                InteractText.SetActive(false);
            }
        }
        else
        {
            currentRaycastTarget = null;
            InteractText.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        HorizontalVelocity();
        VerticalVelocity();
        FinalVelocity();
    }
    private void OnMove(Vector2 directionNormalized)
    {
        absoluteMovementInput = directionNormalized;
    }
    private void OnJump(float context)
    {
        jumpValue = context > 0 ? true : false;
    }
    private void HorizontalVelocity()
    {
        relativeMovementInput = Camera.main.transform.TransformDirection(new Vector3(absoluteMovementInput.x, 0, absoluteMovementInput.y));
        relativeMovementInput.y = 0;
        desiredHorizontalMovement = relativeMovementInput * speed;
    }
    private void VerticalVelocity()
    {
        if(controller.isGrounded && jumpValue)
        {
            currentVerticalVelocity = jumpPower;
        }
        else if (!controller.isGrounded)
        {
            float gravityPerFrame = Physics.gravity.y * Time.deltaTime;
            currentVerticalVelocity += gravityPerFrame;
        }
        jumpValue = false;
    }
    private void FinalVelocity()
    {
        desiredFinalVelocity = new Vector3(desiredHorizontalMovement.x, currentVerticalVelocity, desiredHorizontalMovement.z);
        currentVelocity = Vector3.Lerp(currentVelocity, desiredFinalVelocity, Time.deltaTime * acceleration);
        controller.Move(currentVelocity * Time.deltaTime);
    }

    private void SetSwing(bool state)
    {
        if (state)
        {
            SwingAction.Enable(); 
        }
        else
        {
            SwingAction.Disable();
        }
    }
    private void StartSwing()
    {
        if (chargeCoroutine != null || ShopCamera.activeInHierarchy) return;
        ShovelAnimator.SetTrigger("StartSwing");
        holdingCharge = true;
        chargeCoroutine = StartCoroutine(Charging());
    }
    private void EndSwing()
    {
        holdingCharge = false;
    }
    private IEnumerator Charging()
    {
        Vector3 originalPosition = ShovelObject.transform.localPosition;
        float timer = 0;
        shovelChargeState = 1;
        while(holdingCharge || CheckAnimationName("ShovelCharge"))
        {
            timer += Time.deltaTime;
            if(timer > ChargeTime)
            {
                if(shovelChargeState == 1 && CanChargeTierTwo)
                {
                    shovelChargeState++;
                    timer = 0;
                }
                else if (shovelChargeState == 2 && CanChargeTierThree)
                {
                    shovelChargeState++;
                    timer = 0;
                }
            }
            Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * ChargeMagnitude * shovelChargeState;
            ShovelObject.transform.localPosition = randomOffset;

            yield return null;
        }
        ShovelObject.transform.localPosition = originalPosition;
        ShovelAnimator.SetTrigger("DoSwing");
        if(shovelChargeState == 3)
        {
            ShovelHead.TriggeredThirdCharge?.Invoke();
        }
        chargeCoroutine = null;
    }
    public bool CheckAnimationName(string name)
    {
        AnimatorClipInfo[] clips = ShovelAnimator.GetCurrentAnimatorClipInfo(0);
        if(clips.Length > 0)
        {
            AnimationClip currentClip = clips[0].clip;
            return currentClip.name == name;
        }
        return false;
    }
    private void HandleInteraction()
    {
        if (currentRaycastTarget == null) return;
        if(currentRaycastTarget.TryGetComponent<ShopInteractable>(out ShopInteractable shop))
        {
            shop.StartShop();
        }
    }
    private void SwingUpgrade(int unlockedTier)
    {
        switch (unlockedTier)
        {
            case 2:
                CanChargeTierTwo = true;
                break;
            case 3:
                CanChargeTierThree = true;
                break;
        }
    }
    private void StunIncrease(float amountToIncrease)
    {
        StunTime += amountToIncrease;
    }
    public int GetChargeState() { return shovelChargeState; }
    public float GetStunTime() { return StunTime; }
}
