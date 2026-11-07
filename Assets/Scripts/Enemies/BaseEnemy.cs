
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : Hitable, IHookable
{
    protected enum EnemyState
    {
        Idle,
        Preparing,
        Attack,
        Vulnerable,

        Hooked,
        Stunned,
        Died
    }

    private Vector3 startPosition;
    private Quaternion startRotation;
    protected EnemyState currentState = EnemyState.Idle;
    
    protected EntityStatus status;
    [SerializeField] private float knockbackForce = 5f;

    [SerializeField] private Collider2D[] hurtbox;
    protected Rigidbody2D Rb;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EntityStatus>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        status.TakeDamage(damage);

        
        ApplyKnockback(damageDirection.normalized,knockback);
        

        if (status.GetHealth() <= 0)
            Die();
    }

    private void ApplyKnockback(Vector2 direction, float amount)
    {
        Rb.AddForce(direction * knockbackForce * amount, ForceMode2D.Impulse);
    }
    protected virtual void Die()
    {
        SetState(EnemyState.Died);

        StopAllCoroutines();

        Rb.linearVelocity = Vector2.zero;
        Rb.angularVelocity = 0f;

        foreach (Collider2D col in hurtbox)
        {
            col.enabled = false;
        }
    }

    public virtual void Respawn()
    {
        StopAllCoroutines();

        transform.position = startPosition;
        transform.rotation = startRotation;

        Rb.linearVelocity = Vector2.zero;
        Rb.angularVelocity = 0f;

        Rb.simulated = true;

        foreach (Collider2D col in hurtbox)
        {
            col.enabled = true;
        }

        status.ResetStatus();

        currentState = EnemyState.Idle;
    }

    public virtual void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        StartCoroutine(HookRoutine(origin, direction, offset, pullSpeed));
    }

    protected virtual IEnumerator HookRoutine(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        SetState(EnemyState.Hooked);
        Vector2 destination = origin + direction.normalized * offset;
        float distance = Vector2.Distance(transform.position, destination);
        float pullDuration = distance / pullSpeed;
        OnPullStart();
        yield return PullTowards(origin + direction.normalized * offset, pullDuration);
        SetState(EnemyState.Stunned);
        yield return new WaitForSeconds(2f);
        OnPullEnd();
        SetState(EnemyState.Idle);
    }
    private IEnumerator PullTowards(Vector2 destination, float duration = 0.15f)
    {
        
        Vector2 startPosition = transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            this.transform.position = Vector2.Lerp(startPosition, destination, t);
            yield return null;
        }
        
        this.transform.position = destination;
    }

    public Vector2 GetPosition()
    {
        return Rb.position;
    }

    public virtual void Stun(float duration)
    {
        throw new NotImplementedException();
    }
    public virtual void Stun()
    {
        SetState(EnemyState.Stunned);
    }
    public virtual void DeStun()
    {
        SetState(EnemyState.Idle);
    }
    
    protected virtual void ResetConditions()
    {
        StopAllCoroutines();
        SetState(EnemyState.Idle);
    }

    protected void SetState(EnemyState state)
    {
        if (currentState != EnemyState.Died)
        {
            currentState = state;   
        }
    }

    protected virtual void OnPullStart()
    {
        
    }

    protected virtual void OnPullEnd()
    {
        
    }
}
