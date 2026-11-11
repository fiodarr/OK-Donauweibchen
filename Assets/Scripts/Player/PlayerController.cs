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

    [Header("Cliff Jump")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float jumpVisualHeight = 1f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    
    private int originalLayer;

    private bool isPerformingCliffJump;
    private Vector2 jumpStart;
    private Vector2 jumpDestination;
    private float jumpDuration;
    private float jumpElapsed;
    private Vector3 visualStartPosition;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerComponents = GetComponent<PlayerComponents>();
        solidCollider.forceSendLayers-=Layers.Enemy;
        originalLayer = gameObject.layer;
        
        
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
        if (isPerformingCliffJump)
        {
            UpdateCliffJump();
            return;
        }
        
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
    
    public bool TryStartCliffJump(Vector2 destination, float duration)
    {
        if (isPerformingCliffJump ||
            duration <= 0f ||
            playerComponents.combat.LockMove ||
            playerComponents.status.movementLocked)
            return false;
        
        isPerformingCliffJump = true;
        gameObject.layer = LayerMask.NameToLayer("PlayerAirborne");
        
        jumpStart = rb.position;
        jumpDestination = destination;
        jumpDuration = duration;
        jumpElapsed = 0f;

        visualStartPosition = visualRoot.localPosition;

        rb.linearVelocity = Vector2.zero;

        isMoving = false;

        if (stepsRoutine != null)
        {
            StopCoroutine(stepsRoutine);
            stepsRoutine = null;
        }

        playerComponents.combat.SetExternalActionLock(true);

        return true;
    }
    
    private void UpdateCliffJump()
    {
        jumpElapsed += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(jumpElapsed / jumpDuration);

        float movementT = movementCurve.Evaluate(t);

        rb.MovePosition(Vector2.Lerp(jumpStart, jumpDestination, movementT));

        float height = heightCurve.Evaluate(t) * jumpVisualHeight;

        visualRoot.localPosition = visualStartPosition + Vector3.up * height;

        if (t >= 1f)
        {
            rb.position = jumpDestination;
            visualRoot.localPosition = visualStartPosition;

            isPerformingCliffJump = false;
            playerComponents.combat.SetExternalActionLock(false);
            gameObject.layer = originalLayer;
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
        Debug.Log("Took Damage");
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