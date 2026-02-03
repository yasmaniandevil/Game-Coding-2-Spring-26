using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FPSPlayer : MonoBehaviour
{
    public float walkSpeed = 5;
    public float runSpeed = 10;
    private float jumpForce = 5f;

    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private bool isRunning;
    private float xRot;
    private float yRot;
    private bool isCrouching;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        //optional lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        CameraLook();
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
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void CameraLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        yRot += mouseX;
        transform.rotation = Quaternion.Euler(0f, yRot, 0f);

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }
}
