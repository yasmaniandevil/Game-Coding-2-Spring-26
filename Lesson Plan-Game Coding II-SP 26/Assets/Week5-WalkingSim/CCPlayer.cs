using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    [Header("Interaction")]
    private GameObject currentTarget;
    public Image  reticleImage;
    [FormerlySerializedAs("isInteracting")] public bool interactPressed;
    
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
        
        //find the reticle
       reticleImage = GameObject.Find("Reticle").GetComponent<Image>();
       reticleImage.color = new Color(0, 0, 0, .7f);
    }

    void Start()
    {
        //Debug.Log("isCrouching?: " + isCrouching);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null) return;
        
        HandleLook();
        HandleMovement();
        //HandleCrouch();
        CheckInteract();
        HandleInteract();
        
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
        Debug.Log("isGrounded: " + grounded);
        
        
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
        if (isJumping && grounded)
        {
            //this converts a desired jump height into an initial upward velocity
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            
            isJumping = false;
        }

        //so it only happens once per press
        //on jump is event based, but movement logic runs everyframe
        //without consuming the jump, once you press isJumping it becomes true
        //this would result in repeated jumps from one button press
        //Consume = acknowledge the input once, then immediately clear it so it cannot be reused.
        
        
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

    void CheckInteract()
    {
        //reset reticle image to normal color first
        if(reticleImage != null) reticleImage.color = new Color(0, 0, 0, .7f);
        //make a ray that goes straight out of the camera(center of screen)
        //Start at the camera position
        // Shoot forward from where the camera is looking
        // Check up to 3 units
        //If something is in that path → return information about it
        //players eyesight
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        //asking unity if it hit something within 3 units
        //hit stores what we hit like the collider
        bool didHit = Physics.Raycast(ray, out hit, 3);
        if (!didHit) return;//if we didnt hit anything start here
        //if we hit something tagged interactable
        if (hit.collider.CompareTag("Interactable"))
        {
            //store the object so we can destroy or do whatever when the player clicks
            currentTarget = hit.collider.gameObject;
            if (reticleImage != null)
            {
                reticleImage.color = Color.red;
            }
        }
        
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 3, Color.blue);
    }

    //press mouse
    //is interact = true
    //then update runs HandleInteract which destroys what we clicked on
    //consume means "we handled that click don't reuse it
    void HandleInteract()
    {
        //if the player did not press interact this frame do nothing
        if (!interactPressed) return;
        //consume the input so one click only triggers one interactions
        //this changes next frame
        interactPressed = false;
        if(currentTarget == null) return;
        Destroy(currentTarget);
        //clear target reference after destroying
        currentTarget = null;
        
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

    //think of it like "a click happened this frame" NOT "the player is currently interacting"
    //it is a one frame signal
    //this is event style instead of state style
    //click-> set bool, update-> check bool, reset bool
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) interactPressed = true;
    }
    
    //Tiny mental model
    // interactPressed is a one-time ticket
    // The if line checks: “Do you have a ticket?”
    // If yes, you walk in
    // Then you rip the ticket (interactPressed = false)
    // You still get to do the action inside the room (destroy)
    // Next frame, you need a new ticket (another click).

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Controller collided with " + hit.gameObject.name);
    }
}
