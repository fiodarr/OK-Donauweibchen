using System;
using System.Collections.Generic;
using UnityEngine;
public class Hitbox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] protected Collider2D hitbox;
    public HashSet<Hitable> hitObjects = new HashSet<Hitable>();
    [SerializeField] protected List<Hitable> hitObjectsToIgnore = new List<Hitable>();
    protected DamageData[] currentDamage;
    protected float currentKnockback;
    public Action OnHit;
    protected virtual void Awake()
    {
        hitbox.enabled = false;
        if (spriteRenderer!=null)
        {
        spriteRenderer.enabled = false;
        }
    }

    public void BeginAttack(Vector2 direction, DamageData[] damage, float knockback)
    {
        hitObjects.Clear();

        hitbox.enabled = true;
        if (spriteRenderer!=null)
        {
        spriteRenderer.enabled = true;
        }
        currentDamage = damage;
        currentKnockback = knockback;
    }

    public void EndAttack()
    {
        hitbox.enabled = false;
                if (spriteRenderer!=null)
        {
        spriteRenderer.enabled = false;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if ((Layers.IgnoreHits & (1 << other.gameObject.layer)) != 0)
            return;
        Hitable hitable = other.GetComponentInParent<Hitable>();


        if (hitable == null ||
            hitObjects.Contains(hitable) ||
            hitObjectsToIgnore.Contains(hitable))
            return;

        hitObjects.Add(hitable);

        Vector2 damageDirection =
            (Vector2)(hitable.transform.position - transform.parent.position);

        hitable.TakeDamage(damageDirection, currentDamage, currentKnockback);
        OnHit?.Invoke();
    }

    public void AddToIgnore(Hitable h)
    {
        hitObjectsToIgnore.Add(h);
    }

}