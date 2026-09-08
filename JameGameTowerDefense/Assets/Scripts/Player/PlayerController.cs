using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MovementAction;
    public InputAction JumpAction;

    [SerializeField] private float speed = 10;
    [SerializeField] private float acceleration = 20;
    [SerializeField] private float jumpPower = 10;

    private CharacterController controller;

    private Vector3 absoluteMovementInput = Vector3.zero;
    private Vector3 relativeMovementInput = Vector3.zero;
    private Vector3 desiredHorizontalMovement = Vector3.zero;
    private float currentVerticalVelocity = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredFinalVelocity = Vector3.zero;

    private bool jumpValue;

    private void OnEnable()
    {
        MovementAction.Enable();
        JumpAction.Enable();
    }
    private void OnDisable()
    {
        MovementAction.Disable();
        JumpAction.Disable();
    }
    private void Start()
    {
        MovementAction.performed += context => OnMove(context.ReadValue<Vector2>().normalized);
        MovementAction.canceled += context => OnMove(context.ReadValue<Vector2>().normalized);
        JumpAction.performed += context => OnJump(context.ReadValue<float>());
        
        controller = GetComponent<CharacterController>();
    }

    private void Movement_canceled(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }
    private void Update()
    {
        
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
    private void RotateCamera()
    {

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


}
