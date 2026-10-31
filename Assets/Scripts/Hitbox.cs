using System.Collections.Generic;
using UnityEngine;
public class Hitbox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private BoxCollider2D hitbox;
    private HashSet<Hitable> hitObjects = new HashSet<Hitable>();
    private float currentDamage;

    private void Awake()
    {
        hitbox.isTrigger = true;
        hitbox.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void BeginAttack(Vector2 direction, float damage)
    {
        hitObjects.Clear();

        hitbox.enabled = true;
        spriteRenderer.enabled = true;
        currentDamage = damage;
    }

    public void EndAttack()
    {
        hitbox.enabled = false;
        spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hitable hitable = other.GetComponentInParent<Hitable>();

        if (hitable == null || hitObjects.Contains(hitable))
            return;

        hitObjects.Add(hitable);

        Vector2 damageDirection =
            (Vector2)(hitable.transform.position - transform.parent.position);

        hitable.TakeDamage(damageDirection, currentDamage);
    }
}