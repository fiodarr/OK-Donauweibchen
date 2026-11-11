using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Hitable
{
    private PlayerComponents playerComponents;
    private Camera mainCamera;


    private Coroutine stepsRoutine;

    [SerializeField] private SoundEvent footsteps;

    private bool isMoving = false;


    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;

    public Vector2 FacingDirection { get; private set; }
    public Action OnDamaged;

    [SerializeField] private  Collider2D solidCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerComponents = GetComponent<PlayerComponents>();
        solidCollider.forceSendLayers-=Layers.Enemy;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Gameplay) return;
        
        RefreshAimDirection();
    }

    public void RefreshAimDirection()
    {
        
        Vector2 direction = playerComponents.inputHandler.MoveInput;

        if (direction.sqrMagnitude > 0.001f)
        {
            FacingDirection = direction.normalized;
        }
    }
    
    private void FixedUpdate()
    {
        if (playerComponents.combat.LockMove || playerComponents.status.movementLocked)
        {
            return;
        }

        Vector2 movement = playerComponents.inputHandler.MoveInput.normalized;

        rb.MovePosition(
            rb.position + movement * moveSpeed * Time.fixedDeltaTime
        );
        
        if (movement.magnitude > 0)
        {
            if (stepsRoutine == null)
            {
                stepsRoutine = StartCoroutine(Footsteps());
                isMoving = true;
            }

        }

        else
        {
            isMoving = false;
            if (stepsRoutine != null)
            {
                StopCoroutine(stepsRoutine);
                stepsRoutine = null;
            }
        }


    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        OnDamaged?.Invoke();
        
        playerComponents.status.TakeDamage(damage);
        GamepadVibration.Instance.Vibrate(2,0.1f);
        Vector2 knockbackDirection = damageDirection.normalized;
        rb.AddForce(knockbackDirection * knockback, ForceMode2D.Impulse);
        playerComponents.lantern.Blink();
        PopupManager.Instance.TriggerPopup(PopupEvent.FirstDamageReceived);
    }

    IEnumerator Footsteps()
    {
        while (isMoving){
            RuntimeManager.PlayOneShot(footsteps.nameIndex);
        yield return new WaitForSeconds(0.4f);
        }
        stepsRoutine = null;
    }
}