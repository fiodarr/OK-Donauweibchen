
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [SerializeField] private ShadowCaster2D shadowCaster;
    private Quaternion startRotation;
    protected EnemyState currentState = EnemyState.Idle;
    
    protected EntityStatus status;
    [SerializeField] private float knockbackForce = 5f;

    [SerializeField] private Collider2D[] hurtbox;
    protected Rigidbody2D Rb;
    private SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EntityStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
       
        HitStopManager.Instance.Stop(0.4f, 0f);
        CameraShake.Instance.Shake(0.02f,0.04f);
        GamepadVibration.Instance.Vibrate(2, 0.05f);
        Rb.linearVelocity = Vector2.zero;
        Rb.angularVelocity = 0f;

        foreach (Collider2D col in hurtbox)
        {
            col.enabled = false;
        }

        StartCoroutine(FadeOut());
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

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;

        if (shadowCaster != null)
        {
            shadowCaster.enabled = true;
        }
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
        Debug.Log(currentState);
    }

    protected virtual void OnPullStart()
    {
        
    }

    protected virtual void OnPullEnd()
    {
        
    }

    private IEnumerator FadeOut()
    {
        float duration = 2f;
        float elapsed = 0f;

        Color originalColor = spriteRenderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Color color = originalColor;
            color.a = 1f - Mathf.Clamp01(elapsed / duration);

            spriteRenderer.color = color;

            yield return null;
        }

        Color finalColor = originalColor;
        finalColor.a = 0f;
        spriteRenderer.color = finalColor;
        if (shadowCaster != null)
        {
            shadowCaster.enabled = false;
        }
    }
}
