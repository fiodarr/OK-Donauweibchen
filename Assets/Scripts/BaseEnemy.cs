
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : Hitable
{
    [SerializeField] private float maxHp = 3f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private float currentHp;
    private Rigidbody2D rb;
    private Coroutine knockbackCoroutine;

    private void Awake()
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

        while (elapsed < knockbackDuration)
        {
            float multiplier = 1f - elapsed / knockbackDuration;

            rb.MovePosition(
                rb.position + direction * knockbackForce * multiplier * Time.fixedDeltaTime
            );

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
