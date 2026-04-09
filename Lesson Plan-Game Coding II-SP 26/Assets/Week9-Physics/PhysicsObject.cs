using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Mass and Motion")]
    //how heavy the obj is in kg, affects how much force is needed to move it
    [Range(0.1f, 100f)]
    public float mass = 1f;

    //linear drag how quickly the object slows down in the air 0 = no draf, 10 = very sluggish
    [Range(0f, 10f)]
    public float drag = 0.5f;

    //angular drag how quickly spinning slows down
    [Range(0, 10f)]
    public float angularDrag = .5f;

    [Header("Surface Properties")]
    //bounciness of the surface 0 = no bounce, 1 = perfect bounce, requires physics material on the collider
    [Range(0, 1f)]
    public float bounciness = 0f;
    [Range(0f, 1f)]
    public float friction = 0.6f;

    [Header("Puzzle Properties")]
    //the effective weight used by pressureplate (future script_
    //defaults to mass but can be overrideen
    public float puzzleWeight = -1f; //-1 means "use mass"

    Rigidbody rb;
    PhysicsMaterial physMat; //created at runtime
    public bool isHeld = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ApplyRigidbodySettings();
        ApplySurfaceSettings();
    }
    

    //sets mass and drag directily on the rigidbody comp
    void ApplyRigidbodySettings()
    {
        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
    }

    //[hysics material in unity control bounce and friction
    //we create one at runtime and assign it
    void ApplySurfaceSettings()
    {
        physMat = new PhysicsMaterial(gameObject.name);
        physMat.bounciness = bounciness;
        physMat.dynamicFriction = friction;
        physMat.staticFriction = friction;

        //CombineMode.Maximum means the higher friction of the two
        //colliding objects wins. Good default for solid objects.
        physMat.frictionCombine = PhysicsMaterialCombine.Average;
        physMat.bounceCombine = PhysicsMaterialCombine.Maximum;

        //assign the material to the collider
        Collider col = GetComponent<Collider>();
        if(col != null )
        {
            col.material = physMat;
        }
    }
    // PREVIEW IN EDITOR
    // When you change values in the Inspector during Play mode,
    // this makes them apply immediately without restarting.
    void OnValidate()
    {
        // OnValidate runs in the editor whenever an Inspector value changes
        if (rb != null) ApplyRigidbodySettings();
    }
    
    public float GetWeight()
    {
        if (puzzleWeight >= 0)
            return puzzleWeight;

        return mass;
    }
}
