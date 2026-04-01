using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    //weight settings
    //how much total weight is needed to activate the plate
    public float weightThreadshold = 5f;

    //if true the plate stays activated even after the object is removed
    public bool lockOnActivate = false;

    //event
    //fired when total weight exceeds the threshold
    //different from event action which we were previously doing, unity event needs to be wired in the inspector and is more like how buttons work
    //static event action is just code, doesnt need reference to sender bc static, not as designer friendly
    //fired when total eight exceeds the threshold
    public UnityEvent onActivated;
    //fired when weight drops below the threshold(ignored if lockonactivte is true)
    public UnityEvent onDeactivated;

    //visual feedback
    //optional, the plate mesh that moves down when pressed
    public Transform plate;

    //how far the plate depresses when activated (world units)
    public float pressDepth = 0.05f;

    float currentWeight = 0f;
    bool isActivated = false;
    bool isLocked = false;
    Vector3 plateResetPos;
    Vector3 platePressedPos;

    ObjectGrabber grabber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabber = FindAnyObjectByType<ObjectGrabber>();

        if(plate != null)
        {
            plateResetPos = plate.localPosition;
            platePressedPos = plateResetPos + Vector3.down * pressDepth;
        }
        
    }

    
    //fires when any collider enters the trigger zone
    //we check for physics object to get the weight
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered triger");

        PhysicsObject physicsObj = other.GetComponent<PhysicsObject>();
        if (physicsObj == null) return;

        if (grabber.isHolding) return;

        currentWeight += physicsObj.puzzleWeight;
        Debug.Log($"{other.gameObject.name} entered plate. total weight: {currentWeight}");

        CheckActivation();
       
    }
    
    //fires when collider leaves the trigger zone
    //subtracts that objects weight from the total
    private void OnTriggerExit(Collider other)
    {
        if(isLocked) return;

        PhysicsObject physObj = other.GetComponent<PhysicsObject>();
        if(physObj == null) return;

        currentWeight -= physObj.puzzleWeight;
        currentWeight = Mathf.Max(0f, currentWeight); // prevent negative weight
        Debug.Log($"{other.gameObject.name}. Total weight: {currentWeight}");

        CheckDeactivation();
    }

    //called whenever weight changes, activates if threshold met
    void CheckActivation()
    {
        if(!isActivated && currentWeight >= weightThreadshold)
        {
            isActivated = true;
            if(lockOnActivate) isLocked = true;

            onActivated.Invoke();
            Debug.Log("Pressure Plate Activated!");

            if(plate != null )
            {
                plate.localPosition = platePressedPos;
            }
        }
    }

    //called when weight is removed. deactivtes if below threshold
    void CheckDeactivation()
    {
        if(isActivated && !isLocked && currentWeight < weightThreadshold)
        {
            isActivated = false;
            onDeactivated.Invoke();
            Debug.Log("pressure plate deactivated!");

            if(plate != null)
            {
                plate.localPosition = plateResetPos;
            }
        }
    }
}
