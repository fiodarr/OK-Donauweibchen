using System.Collections.Generic;
using UnityEngine;
public class Hitbox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Collider2D hitbox;
    public HashSet<Hitable> hitObjects = new HashSet<Hitable>();
    [SerializeField] private List<Hitable> hitObjectsToIgnore = new List<Hitable>();
    private DamageData[] currentDamage;
    private float currentKnockback;

    private void Awake()
    {
        hitbox.isTrigger = true;
        hitbox.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void BeginAttack(Vector2 direction, DamageData[] damage, float knockback)
    {
        hitObjects.Clear();

        hitbox.enabled = true;
        spriteRenderer.enabled = true;
        currentDamage = damage;
        currentKnockback = knockback;
    }

    public void EndAttack()
    {
        hitbox.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Hitable hitable = other.GetComponentInParent<Hitable>();

        if (hitable == null ||
            hitable.transform.root == transform.root ||
            hitObjects.Contains(hitable) ||
            hitObjectsToIgnore.Contains(hitable))
            return;

        hitObjects.Add(hitable);

        Vector2 damageDirection =
            (Vector2)(hitable.transform.position - transform.parent.position);

        hitable.TakeDamage(damageDirection, currentDamage, currentKnockback);
    }
}