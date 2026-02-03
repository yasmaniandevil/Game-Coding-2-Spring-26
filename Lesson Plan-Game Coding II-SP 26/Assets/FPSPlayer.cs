using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FPSPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5;
    public float runSpeed = 10;
    private float jumpForce = 5f;
    private bool jumpReady;
    
    
    public Transform cameraTransform;
    public float lookSensitivity = 100f;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isRunning;

    private float yaw;
    private float pitch;
    
    [Header("Ground Check")]
    public LayerMask groundLayer;//what counts as grounding
    public float groundCheckRadius = .5f;//size of sphere
    public float groundCheckDistance = 0.5f;//how far down to check
    public bool isGrounded;
    public Transform groundCheck;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true;
        
        //optional lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraLook();
        CheckGround();
    }

    private void FixedUpdate()
    {
        float currentSpeed;
        
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        Vector3 move = transform.forward * moveInput.y * currentSpeed + 
            transform.right * moveInput.x * currentSpeed;
        
        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        if (jumpReady && isGrounded)
        {
            jumpReady = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void CameraLook()
    {
        if (cameraTransform == null) return;

        // Mouse delta scaled by sensitivity and frame time
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // Horizontal rotation rotates the player body
        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Vertical rotation rotates the camera only
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Prevent flipping

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed) jumpReady = true;
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        // Start the sphere slightly above the player's feet
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        isGrounded = Physics.SphereCast(
            groundCheck.position,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
    }
    private void OnDrawGizmosSelected()
    {
        // Visualize the end position of the spherecast
        Vector3 end = groundCheck.position + Vector3.down * groundCheckDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(end, groundCheckRadius);
    }
    
}
