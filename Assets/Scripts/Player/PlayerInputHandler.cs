using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string grab = "Grab";
    [SerializeField] private string mousePosition = "MousePosition";

    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction rotationAction;
    private InputAction sprintAction;
    private InputAction interactAction;
    private InputAction grabAction;
    private InputAction mousePositionAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public bool JumpHeld { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool InteractHeld { get; private set; }
    public bool GrabHeld { get; private set; }

    // Eventos instantâneos para ações de "click"
    public event Action OnInteractPressed;
    public event Action OnGrabPressed;
    public event Action OnJumpPressed;

    private InputActionMap cachedActionMap;

    private void Awake()
    {
        cachedActionMap = playerControls.FindActionMap(actionMapName);

        if (cachedActionMap == null)
        {
            Debug.LogError($"Action Map '{actionMapName}' not found in Input Action Asset.");
            return;
        }

        movementAction = cachedActionMap.FindAction(movement);
        jumpAction = cachedActionMap.FindAction(jump);
        rotationAction = cachedActionMap.FindAction(rotation);
        sprintAction = cachedActionMap.FindAction(sprint);
        interactAction = cachedActionMap.FindAction(interact);
        grabAction = cachedActionMap.FindAction(grab);
        mousePositionAction = cachedActionMap.FindAction(mousePosition);

        if (movementAction == null || jumpAction == null || rotationAction == null || sprintAction == null || interactAction == null || grabAction == null || mousePositionAction == null)
        {
            Debug.LogError("One or more actions not found in the specified Action Map.");
            return;
        }

        SubscribeActionValuesToInputEvents();
    }

    private void SubscribeActionValuesToInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        mousePositionAction.performed += inputInfo => MousePosition = inputInfo.ReadValue<Vector2>();
        mousePositionAction.canceled += inputInfo => MousePosition = Vector2.zero;

        jumpAction.performed += inputInfo =>
        {
            JumpHeld = true;
            OnJumpPressed?.Invoke();
        };
        jumpAction.canceled += inputInfo => JumpHeld = false;

        sprintAction.performed += inputInfo => SprintHeld = true;
        sprintAction.canceled += inputInfo => SprintHeld = false;

        interactAction.performed += inputInfo =>
        {
            InteractHeld = true;
            OnInteractPressed?.Invoke();
        };
        interactAction.canceled += inputInfo => InteractHeld = false;

        grabAction.performed += inputInfo =>
        {
            GrabHeld = true;
            OnGrabPressed?.Invoke();
        };
        grabAction.canceled += inputInfo => GrabHeld = false;
    }

    private void OnEnable()
    {
        cachedActionMap?.Enable();
    }

    private void OnDisable()
    {
        cachedActionMap?.Disable();
    }
}