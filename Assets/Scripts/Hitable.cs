using UnityEngine;

public abstract class Hitable : MonoBehaviour
{
    private float iTime = 0.1f;

    private float lastHitTime = -Mathf.Infinity;

    public void TakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback)
    {
        if (Time.time < lastHitTime + iTime)
            return;

        lastHitTime = Time.time;

        OnTakeDamage(damageDirection, damage, knockback);
    }

    protected abstract void OnTakeDamage(Vector2 damageDirection, DamageData[] damage, float knockback);
}