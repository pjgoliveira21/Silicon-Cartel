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
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool GrabTriggered { get; private set; }
    public Vector2 MousePosition { get; private set; }

    private void Awake()
    {
        InputActionMap actionMap = playerControls.FindActionMap(actionMapName);
        if (actionMap == null)
        {
            Debug.LogError($"Action Map '{actionMapName}' not found in Input Action Asset.");
            return;
        }

        movementAction = actionMap.FindAction(movement);
        jumpAction = actionMap.FindAction(jump);
        rotationAction = actionMap.FindAction(rotation);
        sprintAction = actionMap.FindAction(sprint);
        interactAction = actionMap.FindAction(interact);
        grabAction = actionMap.FindAction(grab);
        mousePositionAction = actionMap.FindAction(mousePosition);

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

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        sprintAction.performed += inputInfo => SprintTriggered = true;
        sprintAction.canceled += inputInfo => SprintTriggered = false;

        interactAction.performed += inputInfo => InteractTriggered = true;
        interactAction.canceled += inputInfo => InteractTriggered = false;

        mousePositionAction.performed += inputInfo => MousePosition = inputInfo.ReadValue<Vector2>();
        mousePositionAction.canceled += inputInfo => MousePosition = Vector2.zero;

        grabAction.performed += inputInfo => GrabTriggered = true;
        grabAction.canceled += inputInfo => GrabTriggered = false;
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName)?.Enable();
    }

    private void OnDisable()
    {
        playerControls.FindActionMap(actionMapName)?.Disable();
    }   
}
