
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : Hitable, IHookable
{
    protected enum EnemyState
    {
        Idle,
        Preparing,
        Attack,
        Stunned
    }
    
    
    protected EnemyState CurrentState = EnemyState.Idle;
    
    EntityStatus status;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;
    protected Rigidbody2D Rb;
    private Coroutine knockbackCoroutine;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        status = GetComponent<EntityStatus>();
    }

    protected override void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        status.TakeDamage(damage);

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            ApplyKnockback(damageDirection.normalized)
        );

        if (status.GetHealth() <= 0)
            Die();
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        float elapsed = 0f;

        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(Layers.Walls);
        filter.useTriggers = false;

        RaycastHit2D[] hits = new RaycastHit2D[1];

        while (elapsed < knockbackDuration)
        {
            float multiplier = 1f - elapsed / knockbackDuration;
            float distance = knockbackForce * multiplier * Time.fixedDeltaTime;

            int hitCount = Rb.Cast(direction, filter, hits, distance);

            if (hitCount == 0)
            {
                Rb.MovePosition(Rb.position + direction * distance);
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        knockbackCoroutine = null;
    }
    private void Die()
    {
        Destroy(gameObject);
    }

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        ResetConditions();
        Vector2 destination = origin + direction.normalized * offset;
        float distance = Vector2.Distance(transform.position, destination);
        float pullDuration = distance / pullSpeed;
        StartCoroutine(PullTowards(origin + direction.normalized * offset, pullDuration));
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

    protected virtual void ResetConditions()
    {
        StopAllCoroutines();
        knockbackCoroutine = null;
        CurrentState = EnemyState.Idle;
    }
}
