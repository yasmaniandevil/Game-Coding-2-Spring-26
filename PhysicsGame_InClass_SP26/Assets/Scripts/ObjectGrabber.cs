using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("How far away the player can grab objects from")]
    public float grabRange = 4;

    [Tooltip("How fast the held object moves to the hold point. higher = snappier")]
    public float holdSmoothing = 15f;

    //the point in front of the camera where the object is held
    public Transform holdPoint;

    //how muuch force is applied when throwing
    public float throwForce = 15f;

    private Rigidbody heldObject;
    private bool isHolding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FixedUpdate()
    {
        //fixed update runs on an interval schedule
        //we move the held object here so it stays smooth and physics is accurate
        if (isHolding && heldObject != null) MoveHeldObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TryGrab()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //drawing the ray for debugging
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.yellow, 0.5f);

        if(Physics.Raycast(ray, out hit, grabRange))
        {
            //check if the hit object has the interactable marker script
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if(interactable != null)
            {
                //get rigidbody so we can move it with physics
                heldObject = hit.collider.GetComponent<Rigidbody>();
                if(heldObject != null)
                {
                    //disable gravity so it floats in front of us while held
                    heldObject.useGravity = false;

                    //freeze rotation so it doesnt spin around while carried
                    heldObject.freezeRotation = true;

                    //zero out any existing velocity so it doesnt fly away
                    heldObject.linearVelocity = Vector3.zero;
                    heldObject.angularVelocity = Vector3.zero;

                    isHolding = true;
                    Debug.Log($"Grabbed {heldObject.name}");
                }
            }


        }
    }

    //called ebery fixed update while holding an object
    //smoothly moves the rigidbody toward the hold point
    void MoveHeldObject()
    {
        Vector3 targetPos = holdPoint.position;
        Vector3 currentPos = heldObject.position;

        //smoothly interpolate toward the hold point
        //move position respects physics collision (object wont clip thru wall)
        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, holdSmoothing * Time.fixedDeltaTime);

        heldObject.MovePosition(newPos);
    }

    //drop 
    //releases the object and restores normal physics behavior
    void DropObject()
    {
        if (heldObject == null) return; //if we arent holding anything, exit this function

        //re-enable gravity and rotation
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        heldObject = null; //clearing it
        isHolding = false;
        Debug.Log("Dropped Object");
    }

    //releases the obj and launches it forward using addforce
    void ThrowObject()
    {
        if (heldObject == null) return;

        //re-enable physics
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        //apply force in the direction the camera is facing
        //forcemode.impulse applies the force INSTANTLY like a punch
        //as opposed to forcemode.force which applies gradually over time
        heldObject.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHolding = false;
        Debug.Log("Threw Object"); 
    }

    public void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (isHolding) DropObject();
        else TryGrab();
    }

    public void OnThrowPerformed(InputAction.CallbackContext context)
    {
        if (isHolding) ThrowObject();
       
    }
}
