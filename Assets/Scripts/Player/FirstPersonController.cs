using System;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookLimit = 85.0f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private Animator animator;

    private Vector3 currentMovement;
    private float verticalLookRotation;
    private float CurrentSpeed => walkSpeed * (inputHandler.SprintTriggered ? sprintMultiplier : 1);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleLooking();
    }

    private Vector3 CalculateWorldDirecton()
    {
        Vector3 inputDirection = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f; // Small downward force to keep the player grounded

            if (inputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
                animator.SetTrigger("Jumping");
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirecton();
        Vector3 horizontalMovement = worldDirection * CurrentSpeed;
        currentMovement.x = horizontalMovement.x;
        currentMovement.z = horizontalMovement.z;
        
        if(currentMovement.magnitude > 1f)
        {

            animator.SetBool(inputHandler.SprintTriggered ? "Running" : "Walking", true);
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetBool("Walking", false);
        }

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotation)
    {
        transform.Rotate(0, rotation, 0);
    }

    private void ApplyVerticalRotation(float rotation)
    {
        verticalLookRotation = Mathf.Clamp(verticalLookRotation - rotation, -upDownLookLimit, upDownLookLimit);
        cameraTransform.transform.localRotation = Quaternion.Euler(verticalLookRotation, 0, 0);
    }

    private void HandleLooking()
    {
        Vector2 rotationInput = inputHandler.RotationInput * mouseSensitivity;
        ApplyHorizontalRotation(rotationInput.x);
        ApplyVerticalRotation(rotationInput.y);
    }
}
