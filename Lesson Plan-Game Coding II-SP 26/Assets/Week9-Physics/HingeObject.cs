using UnityEngine;

public class HingeObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Angle Limits")]
    [Tooltip("The minimum angle the hinge can rotate to (e.g. 0 = closed)")]
    public float minAngle = 0f;

    [Tooltip("The maximum angle the hinge can rotate to (e.g. 90 = fully open)")]
    public float maxAngle = 90f;

    [Header("Spring Settings")]
    [Tooltip("If true, the hinge will spring back toward the rest angle when released")]
    public bool useSpring = true;

    [Tooltip("The angle the spring tries to return to")]
    public float springTargetAngle = 0f;

    [Tooltip("How strong the spring force is")]
    public float springForce = 50f;

    [Tooltip("How much the spring dampens (reduces oscillation)")]
    public float springDamper = 5f;

    [Header("Puzzle Events")]
    [Tooltip("Fired when the hinge reaches or passes the max angle")]
    public UnityEngine.Events.UnityEvent OnReachedMax;

    [Tooltip("Fired when the hinge returns to or passes the min angle")]
    public UnityEngine.Events.UnityEvent OnReachedMin;

    [Tooltip("How close to the limit angle before the event fires (degrees)")]
    public float eventThreshold = 5f;

    // ---- Private refs ----
    private HingeJoint hinge;
    private bool maxEventFired = false;
    private bool minEventFired = false;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        ConfigureHinge();
    }

    // -------------------------------------------------------
    // CONFIGURE HINGE
    // Sets up joint limits and spring through code.
    // Students can see exactly what each property does.
    // -------------------------------------------------------
    void ConfigureHinge()
    {
        // --- Limits ---
        // JointLimits is a struct — we have to set all fields then assign it back
        JointLimits limits = hinge.limits;
        limits.min = minAngle;
        limits.max = maxAngle;
        limits.bounciness = 0f;          // no bounce at the limit
        limits.bounceMinVelocity = 0.2f;
        hinge.limits = limits;
        hinge.useLimits = true;

        // --- Spring ---
        if (useSpring)
        {
            JointSpring spring = hinge.spring;
            spring.targetPosition = springTargetAngle;
            spring.spring = springForce;
            spring.damper = springDamper;
            hinge.spring = spring;
            hinge.useSpring = true;
        }
        else
        {
            hinge.useSpring = false;
        }
    }

    void Update()
    {
        // Check if we've hit the limits and should fire puzzle events
        float currentAngle = hinge.angle;

        if (!maxEventFired && currentAngle >= maxAngle - eventThreshold)
        {
            maxEventFired = true;
            minEventFired = false;
            OnReachedMax?.Invoke();
            Debug.Log($"{gameObject.name} hinge reached max angle");
        }
        else if (!minEventFired && currentAngle <= minAngle + eventThreshold)
        {
            minEventFired = true;
            maxEventFired = false;
            OnReachedMin?.Invoke();
            Debug.Log($"{gameObject.name} hinge reached min angle");
        }
    }

    // -------------------------------------------------------
    // PUBLIC METHODS
    // Can be wired to UnityEvents or called directly.
    // -------------------------------------------------------

    // Drive the hinge to its max angle using the motor
    public void DriveToMax()
    {
        SetMotorTarget(maxAngle);
    }

    // Drive the hinge back to its min angle
    public void DriveToMin()
    {
        SetMotorTarget(minAngle);
    }

    void SetMotorTarget(float targetAngle)
    {
        JointMotor motor = hinge.motor;
        // Motor velocity direction determines which way it moves
        motor.targetVelocity = (targetAngle > hinge.angle) ? 50f : -50f;
        motor.force = 100f;
        motor.freeSpin = false;
        hinge.motor = motor;
        hinge.useMotor = true;
    }
}
