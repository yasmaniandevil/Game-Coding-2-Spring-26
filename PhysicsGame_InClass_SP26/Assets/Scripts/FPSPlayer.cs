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
        //grab rigidbody off of played
        rb = GetComponent<Rigidbody>();
        
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

    //fixed update is not every frame like Update, instead it runs at scheduled consistent intervals
    //this is better for when we are doing phsyics
    private void FixedUpdate()
    {
        float currentSpeed;
        //if running is true or false update the speed either to walk or run speed
        if (isRunning)
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        //once current speed gets updated it sends it here
        //movement math
        Vector3 move = transform.forward * moveInput.y * currentSpeed + 
            transform.right * moveInput.x * currentSpeed;
        
        //applying our move vector above to change our rigidbodies velocity
        //keep velocity the same on the y
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        //if jump ready is true (depending on our input)
        //and the player is grounded
        if (jumpReady && isGrounded)
        {
            //turn jump ready to false so we only jump once, it gets back updated to true in OnJump Function
            jumpReady = false;
            //add force to our rigidbody
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    //reads the value of our mouse as a vector2 (x,y) stores it into our moveinput var
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void CameraLook()
    {
        //if we do not have a camera assigned, exit the function here (do not do the rest of it)
        if (cameraTransform == null) return;
        
        //take the input of our mouse on the x and y, multiples it by how fast we want our camera to move, multiply by framerate
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * lookSensitivity * Time.deltaTime;

        // Horizontal rotation rotates the player body
        //yaw means left to right
        yaw += mouseX;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Vertical rotation rotates the camera only
        //pitch means up to down
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Prevent flipping

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    //if we press the jump button (space bar)
    //then we want to set kump readu to true
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed) jumpReady = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
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
