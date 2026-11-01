using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Hitable
{
    private PlayerInputHandler inputHandler;
    private PlayerCombat combat;
    private Camera mainCamera;
    private EntityStatus status;

    private Coroutine stepsRoutine;

    [SerializeField] private SoundEvent footsteps;

    private bool isMoving = false;

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;

    public Vector2 FacingDirection { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = GetComponent<PlayerInputHandler>();
        combat = GetComponent<PlayerCombat>();
        status = GetComponent<EntityStatus>();

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
        if (combat.LockMove)
        {
            return;
        }

        Vector2 movement = inputHandler.MoveInput.normalized;

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );

        if (movement.x > 0 || movement.y > 0)
        {
            isMoving = true;
            if (stepsRoutine == null)
                stepsRoutine = StartCoroutine(Footsteps());
        }
        else
        {
            isMoving = false;
        }
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        status.TakeDamage(damage);
        
        Vector2 knockbackDirection = damageDirection.normalized;
        rb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
    }

    IEnumerator Footsteps()
    {
        while (isMoving){
        footsteps.Play();
        yield return new WaitForSeconds(0.4f);
        }
    }
}