using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CCPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5;
    public float runSpeed = 9;
    public float crouchSpeed;
    [FormerlySerializedAs("jumpForce")] public float jumpHeight;
    
    [Header("Look")]
    public Transform cameraTransform;
    public float lookSensitivity = 1f;

    public float standingHeight = 2f;
    public float crouchHeight = 1.2f;
    public float crouchTransitionSpeed = 12f;

    private CharacterController cc;

    private Vector2 moveInput;
    private Vector2 lookInput;
    
    private float verticalVelocity; //current upward/downward speed
    private float gravity = -20f; //constat downward acceleration
    private float pitch; //camera vertical rotation
    
    public bool isCrouching;
    private bool isRunning;
    private bool isJumping;
    
    // Start is called before the first frame update
    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            Debug.Log("no camera in the inspector");
        }
        
        //optional
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        /*cc.height = standingHeight;
        cc.center = new Vector3(0, standingHeight * 0.5f, 0);
        isCrouching = false;*/
    }

    void Start()
    {
        Debug.Log("isCrouching?: " + isCrouching);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null) return;
        
        HandleLook();
        HandleMovement();
        //HandleCrouch();
        
    }

    private void HandleLook()
    {
        //mouse delta is already per frame so we dont need to multiple by times.deltatime
        //multiply our input by the mouse sensatitivity 
        //horizontal mouse movement rotates the player body
        float yaw = lookInput.x * lookSensitivity;
        //vertical mouse movement rotates the camera
        float pitchDelta = lookInput.y * lookSensitivity;
        
        transform.Rotate(Vector3.up * yaw);

        //accumulate vertical rotation
        pitch -= pitchDelta;
        //clamp so we dont flip upside down
        pitch = Mathf.Clamp(pitch, -90, 90);
        
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);

    }

    private void HandleMovement()
    {
        //cc build in grounded check
        bool grounded = cc.isGrounded;
        
        //if grounded and falling force a small downard velocity
        //this keeps controller snapped to ground
        if (grounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; //small stick to the ground
        }
        
        //first we set the current speed to the walk speed
        float currentSpeed = walkSpeed;
        //choose speed based on state
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if(isRunning)
        {
            currentSpeed = runSpeed;
        }
        
        //convert input into world space movement
        Vector3 move = transform.right * moveInput.x * currentSpeed + 
                       transform.forward * moveInput.y * currentSpeed;

        //only allowed if grounded
        if (isJumping && grounded && !isCrouching)
        {
            //this converts a desired jump height into an initial upward velocity
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //so it only happens once per press
        //on jump is event based, but movement logic runs everyframe
        //without consuming the jump, once you press isJumping it becomes true
        //this would result in repeated jumps from one button press
        //Consume = acknowledge the input once, then immediately clear it so it cannot be reused.
        
        isJumping = false;
        
        //apply gravity every frame
        verticalVelocity += gravity * Time.deltaTime;
        
        //convert vertical velocity into movement vector
        Vector3 velocity = Vector3.up * verticalVelocity;
        //now we are finally moving it
        cc.Move((move + velocity) * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        float targetHeight = standingHeight;
        if (isCrouching)
        {
            targetHeight = crouchHeight;
        }
        else
        {
            targetHeight = standingHeight;
        }
        
        //smoothly changed cc height
        cc.height = Mathf.Lerp(cc.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        
        //keep feet planted while changing height
        Vector3 center = cc.center;
        center.y = cc.height * 0.5f;
        cc.center = center;
    }

    //input system calbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed) isJumping = true;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValueAsButton();
    }
}
