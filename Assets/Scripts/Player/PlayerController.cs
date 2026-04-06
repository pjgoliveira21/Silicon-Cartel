using UnityEngine.InputSystem;
using UnityEngine;
using System;


public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.8f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private Vector2 moveInput;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + moveInput);
        animator.SetBool("Walking", moveInput.magnitude > 0);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = jumpHeight;
            Debug.Log("Jump!");
            velocity.y = MathF.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("Jumping", true);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveSpeed *= 2;
            animator.SetBool("Running", true);
        }
        else if (context.canceled)
        {
            moveSpeed /= 2;
            animator.SetBool("Running", false);
        }
    }
    
    // Update is called once per frame
    void Update()
    {

        Vector3 move = new(moveInput.x, 0, moveInput.y);
        controller.Move(move * moveSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
