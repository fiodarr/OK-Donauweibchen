using UnityEngine;

public abstract class Hitable : MonoBehaviour
{
    [SerializeField] private float iTime = 0.2f;

    private float lastHitTime = -Mathf.Infinity;

    public void TakeDamage(Vector2 damageDirection, float damage, float knockback)
    {
        if (Time.time < lastHitTime + iTime)
            return;

        lastHitTime = Time.time;

        OnTakeDamage(damageDirection, damage, knockback);
    }

    protected abstract void OnTakeDamage(Vector2 damageDirection, float damage, float knockback);
}