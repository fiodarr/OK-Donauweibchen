using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerInputHandler inputHandler;
    private PlayerCombat combat;
    private Camera mainCamera;

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;

    public Vector2 FacingDirection { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        combat = GetComponent<PlayerCombat>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        RefreshAimDirection();
    }

    public void RefreshAimDirection()
    {
        Vector2 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(inputHandler.MousePosition);

        Vector2 direction =
            mouseWorldPosition - rb.position;

        if (direction.sqrMagnitude > 0.001f)
            FacingDirection = direction.normalized;
    }

    private void FixedUpdate()
    {
        if (combat.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 movement = inputHandler.MoveInput.normalized;

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }
}