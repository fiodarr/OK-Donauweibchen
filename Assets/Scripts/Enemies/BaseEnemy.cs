
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : Hitable
{
    [SerializeField] private float maxHp = 3f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private float currentHp;
    protected Rigidbody2D rb;
    private Coroutine knockbackCoroutine;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;
    }

    public override void TakeDamage(Vector2 damageDirection, float damage)
    {
        currentHp -= damage;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            ApplyKnockback(damageDirection.normalized)
        );

        if (currentHp <= 0)
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

            int hitCount = rb.Cast(direction, filter, hits, distance);

            if (hitCount == 0)
            {
                rb.MovePosition(rb.position + direction * distance);
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
}
