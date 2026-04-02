using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    //weight settings
    //how much total weight is needed to activate the plate
    public float weightThreshold = 5f;

    //if true the plate stays activated even after the object is removed
    public bool lockOnActivate = false;

    //event
    //fired when total weight exceeds the threshold
    //diff from event action which we were previously doing, unity event needs to be wired in the inspector and is more like buttons
    //static event action is just code, doesnt need ref to sender bc static, not as designer friendly
    //fired when total exceeds the threshold
    public UnityEvent onActivated;
    //fired when weight drops below the threshold (ignored if lockonactivate is true)
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

    HashSet<PhysicsObjects> objectsOnPlate = new HashSet<PhysicsObjects>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(plate !=null)
        {
            //storing where our plate is
            plateResetPos = plate.localPosition;
            //taking that and moving it down
            platePressedPos = plateResetPos + Vector3.down * pressDepth;
        }
    }

    //fires when any collider enters the trigger zone
    //we check for physics obj to get the weight
    private void OnTriggerEnter(Collider other)
    {
       PhysicsObjects physOb = other.GetComponent<PhysicsObjects>();
       if (physOb == null) return;

       if (physOb.isHeld) return; //so it doesnt go off when youre just holding it in the trigger area

        //first simple version
        /*currentWeight += physOb.puzzleWeight;
        Debug.Log($"{other.gameObject.name} entered plate. total weight: {currentWeight}");
        CheckActivation();*/

       //this is instead adding it to a list at first just to make sure nothing gets double activated
       //the above works too
        if (objectsOnPlate.Add(physOb))
        {
            currentWeight += physOb.puzzleWeight;
            CheckActivation();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        PhysicsObjects physicsObj = other.GetComponent<PhysicsObjects>();
        if (physicsObj == null) return;

        //ifnore if still being held
        if(physicsObj.isHeld) return;

        if(objectsOnPlate.Add(physicsObj))
        {
            currentWeight += physicsObj.puzzleWeight;
            CheckActivation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(isLocked) return;
        PhysicsObjects physicsObj = other.GetComponent<PhysicsObjects>();
        if (physicsObj == null) return;

        if (objectsOnPlate.Remove(physicsObj))
        {
            currentWeight -= physicsObj.puzzleWeight;
            currentWeight = Mathf.Max(0f, currentWeight);
            CheckDeactivation();
        }
    }


    //called whenever weight changes, activates if threshold is met
    void CheckActivation()
    {
        if(!isActivated && currentWeight >= weightThreshold)
        {
            isActivated = true;
            if(lockOnActivate) isLocked = true;

            //calls it for whatever is listening to it
            onActivated.Invoke();
            Debug.Log("Pressure plate is activated");

            if(plate != null)
            {
                //after its activated move the plate
                plate.localPosition = platePressedPos;
            }
        }
    }

    //call this when weight is removed deactivates if below threshold
    void CheckDeactivation()
    {
        if(isActivated && !isLocked && currentWeight < weightThreshold)
        {
            isActivated = false;
            onDeactivated.Invoke();
            Debug.Log("pressure plate is deactivated");

            if(plate != null)
            {
                plate.localPosition = plateResetPos;
            }
        }
    }
}
