using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EntityStatus))]
public class BaseEnemy : Hitable, IHookable, IIntensityEmitter
{
    protected enum EnemyState
    {
        Idle,
        Preparing,
        Attack,
        Vulnerable,
        Hooked,
        Stunned,
        Pingponged,
        Died
    }

    private Vector3 startPosition;
    [SerializeField] private ShadowCaster2D shadowCaster;
    private Quaternion startRotation;
    protected EnemyState currentState = EnemyState.Idle;
    
    protected EntityStatus status;
    [SerializeField] private float knockbackForce = 5f;
    
    [SerializeField] private float knockbackPingPongMultiplier = 2f;
    [SerializeField] private float knockbackPingPongTime = 1f;

    [SerializeField] private EventReference pingPongSound;
    
    [SerializeField] private Collider2D[] hurtbox;
    protected Rigidbody2D Rb;
    private SpriteRenderer spriteRenderer;
    
    private Coroutine stunCoroutine = null;
    private Coroutine pingpongCoroutine = null;

    private DamageData pingPongAttack;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EntityStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        pingPongAttack = new DamageData()
        {
            amount = status.GetMaxHealth() * 0.4f,
            tag = AttackDataTag.Default,
            type = DamageType.Physical
        };
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        status.TakeDamage(damage);
        if (status.GetHealth() <= 0)
            Die();
        
        if (currentState == EnemyState.Stunned 
            && damage.Any(d => d.tag == AttackDataTag.PingPongTrigger))
        {
            RuntimeManager.PlayOneShot(pingPongSound);
            StartPingPong(knockbackPingPongTime);
        }
        
        ApplyKnockback(damageDirection.normalized,knockback);
        
        
    }

    private void ApplyKnockback(Vector2 direction, float amount)
    {
        Rb.AddForce(direction 
                    * knockbackForce * amount 
                    * (currentState == EnemyState.Pingponged ? knockbackPingPongMultiplier : 1), 
            ForceMode2D.Impulse);
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
        
        PopupManager.Instance.TriggerPopup(PopupEvent.FirstHooked);
        stunCoroutine = StartCoroutine(StunRoutine(2f));
        yield return stunCoroutine;
        Debug.Log("STUN ENDED");
        OnPullEnd();
    }

    private IEnumerator StunRoutine(float duration)
    {
        SetState(EnemyState.Stunned);
        yield return new WaitForSeconds(duration);
        SetState(EnemyState.Idle);
        OnPullEnd();
        stunCoroutine = null;
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
        Debug.Log("OnPullEnd base!!");
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

    protected virtual void StartPingPong(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = null;
        }
        pingpongCoroutine = StartCoroutine(PingPongRoutine(duration));
    }

    private IEnumerator PingPongRoutine(float duration)
    {
        
        Debug.Log("StartPingPong");
        currentState = EnemyState.Pingponged;
        yield return new WaitForSeconds(duration);
        pingpongCoroutine = null;
        currentState = EnemyState.Idle;
        OnPullEnd();
        Debug.Log("EndPingPong");
    }

    public IntensityMonitor.IntensityLevel GetIntensity()
    {
        return IntensityMonitor.IntensityLevel.Low;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (currentState != EnemyState.Pingponged) return;

        Vector2 direction;
        
        if (((1 << collision.gameObject.layer) & Layers.Walls) != 0)
        {
            direction = collision.contacts[0].normal;
            this.TakeDamage(direction,new[]{pingPongAttack},15f);
            StopPingPong(0.5f);
            return;
        }
        
        if (!collision.gameObject.TryGetComponent(out Hitable hittable)) return;
        if (hittable.gameObject.layer == Layers.IgnoreHits || hittable.gameObject.TryGetComponent(out PlayerController playerController)) return;
        
        direction = (transform.position - collision.transform.position).normalized;;//calculate direction opposing of hitted object
        this.TakeDamage(direction,new [] {pingPongAttack},10f);
        hittable.TakeDamage(-direction,new[]{pingPongAttack},10f);
        StopPingPong(0.1f);
    }

    private void StopPingPong(float recoveryStunTime = 0f)
    {
        if (pingpongCoroutine != null) 
        {
            StopCoroutine(pingpongCoroutine);
        }

        if (recoveryStunTime > 0f)
        {
            stunCoroutine = StartCoroutine(StunRoutine(recoveryStunTime));
        }
        else
        {
            currentState = EnemyState.Idle;
            OnPullEnd();
        }
    }
}
