using UnityEngine;

public class LanternPendulum : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pivot;

    [Header("Physics")]
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float accelerationInfluence = 1.5f;
    [SerializeField] private float stiffness = 25f;
    [SerializeField] private float damping = 7f;
    [SerializeField] private float maxAngle = 40f;

    [Header("Acceleration Smoothing")]
    [SerializeField] private float accelerationSmoothing = 8f;

    private Vector2 previousPosition;
    private Vector2 previousVelocity;
    private Vector2 smoothedAcceleration;

    private float angle;
    private float angularVelocity;

    private Quaternion initialRotation;

    private void Start()
    {
        if (pivot == null)
        {
            enabled = false;
            return;
        }

        previousPosition = pivot.position;
        initialRotation = pivot.localRotation;
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;

        if (dt <= 0f)
            return;

        // Measure pivot velocity
        Vector2 currentPosition = pivot.position;

        Vector2 velocity =
            (currentPosition - previousPosition) / dt;

        // Measure pivot acceleration
        Vector2 acceleration =
            (velocity - previousVelocity) / dt;

        previousPosition = currentPosition;
        previousVelocity = velocity;

        // Smooth acceleration
        float smoothing = 1f - Mathf.Exp(
            -accelerationSmoothing * dt
        );

        smoothedAcceleration = Vector2.Lerp(
            smoothedAcceleration,
            acceleration,
            smoothing
        );

        // Calculate target angle
        float targetAngle = Mathf.Atan2(
            -smoothedAcceleration.x * accelerationInfluence,
            gravity
        ) * Mathf.Rad2Deg;

        targetAngle = Mathf.Clamp(
            targetAngle,
            -maxAngle,
            maxAngle
        );

        // Spring-damper simulation
        float angularAcceleration =
            (targetAngle - angle) * stiffness
            - angularVelocity * damping;

        angularVelocity += angularAcceleration * dt;
        angle += angularVelocity * dt;

        // Clamp rotation
        if (angle > maxAngle || angle < -maxAngle)
        {
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
            angularVelocity = 0f;
        }

        // Apply rotation around pivot
        pivot.localRotation =
            initialRotation *
            Quaternion.Euler(0f, 0f, angle);
    }
}