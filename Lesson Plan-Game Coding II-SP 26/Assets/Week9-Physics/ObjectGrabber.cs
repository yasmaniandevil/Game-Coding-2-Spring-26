using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("How far away the player can grab objects from")]
    public float grabRange = 4f;

    [Tooltip("How fast the held object moves to the hold point (higher = snappier")]
    public float holdSmoothing = 15f;

    //the point in front of the camera where the object is held
    public Transform holdPoint;

    [Header("Throw Settings")]
    //how much force is applied when throwing
    public float throwForce = 15f;

    Rigidbody heldObject; //the rigidbody we are currently holding
    public bool isHolding = false; //are we currently holding something?

    //for later after we do base script
    //track the currently highlighted object so we can unhighlight it
    InteractableObject currentHighlight;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        //run the detection raycast every frame to update the highlight
        //this is seperate from the grab raycast- it just checks what the player is looking at and highlight/unhighlights accordingly
        //this is for later
        UpdateHighlight();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //fixed update runs in sync with the physics engine
        //we move the held object here so it stays smooth and physics accurate
        if(isHolding && heldObject != null) MoveHeldObject();
    }

    void TryGrab()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //draw the ray in the scene view for debugging
        //Debug.DrawRay(transform.position, transform.forward * grabRange, Color.green, 0.5f);

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            //check if the hit object has the grabbable marker script
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                //get rigidbody so we can move it with physics
                heldObject = hit.collider.GetComponent<Rigidbody>();

                if (heldObject != null)
                {
                    //disable gravity so it floats in front of us while held
                    heldObject.useGravity = false;

                    //freeze rotate so it doesnt spin around while carried
                    heldObject.freezeRotation = true;

                    //zero out any existing velocity so it doesnt fly away
                    heldObject.linearVelocity = Vector3.zero;
                    heldObject.angularVelocity = Vector3.zero;

                    //add later
                    //unhighlight when grabbed- object is now in hand
                    interactable.Unhighlight();
                    currentHighlight = null;

                    isHolding = true;
                    //Debug.Log($"Grabbed {heldObject.gameObject.name}");
                }
            }
        }

    }
    //called every fixed update while holding an object
    //smoothly moves the rigidbody toward the hold point
    void MoveHeldObject()
    {
        Vector3 targetPos = holdPoint.position;
        Vector3 currentPos = heldObject.position;

        //smoothly interpolate toward the hold point
        //move position respects physics collision (object wont clip thru walls)
        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, holdSmoothing * Time.fixedDeltaTime);

        heldObject.MovePosition(newPos);
    }

    //drop
    //releases the object and restores normal physics behavior
    void DropObject()
    {
        if (heldObject == null) return;

        //re-enable gravity and rotation
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        //zero velocity so it doesnt launch away on drop
        heldObject.linearVelocity = Vector3.zero;
        heldObject.angularVelocity = Vector3.zero;

        heldObject = null;
        isHolding = false;

        

    }

    //releases the object and launches it forward using addforce
    void ThrowObject()
    {
        
        if(heldObject == null) return;

        //re-enable physics first
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        //apply force in the cirection the camera is facing
        //forcemode.impulse applies the force instantly like a punch
        //as oposed to forcemode.force which applies gradually over time
        heldObject.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHolding = false;
        //Debug.Log("Threw Object");

    }

    public void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if (isHolding) 
        {
            DropObject();


        }
        else
        {
            TryGrab();
            
        }

        
    }

    public void OnThrowPerformed(InputAction.CallbackContext context)
    {
        if ((!context.performed))
        {
            return;
        }
        if (isHolding) ThrowObject();
    }

    void UpdateHighlight()
    {
        //dont change highlights while holding an object
        if(isHolding) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        //draw the detection ray in scene view (editor only)
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.red);


        if(Physics.Raycast(ray, out hit, grabRange))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            //Debug.Log("hit interactable");
            if ((interactable != null))
            {
                //Debug.Log("current interactable: " +  interactable);
                //Debug.Log("current highlight: " + currentHighlight);
                //if we now looking at different object unhighlight the old one
                if(currentHighlight != null && currentHighlight != interactable)
                {
                    currentHighlight.Unhighlight();
                    //Debug.Log("call unhighlight");
                   
                }

                //highlight the new obj
                interactable.Highlight();
                //Debug.Log("call highlight");
                currentHighlight = interactable;
                return;

            }

            //raycast hit nothing interactable - clear the highlight
            if(currentHighlight != null)
            {
                //Debug.Log("did not hit interactable");
                currentHighlight.Unhighlight();
                currentHighlight = null;
            }
        }
        
    }
}
