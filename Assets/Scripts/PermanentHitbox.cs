using UnityEngine;

public class PermanentHitbox : Hitbox
{
    [SerializeField] private DamageData[] manualDamage;
    [SerializeField] private float manualKnockback;
    [SerializeField] private bool destroyOnHit;
    public Vector2 direction;
    private Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        hitbox.enabled = true;
        currentDamage = manualDamage;
        currentKnockback = manualKnockback;
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        if ((Layers.IgnoreHits & (1 << other.gameObject.layer)) != 0)
            return;
        Hitable hitable = other.GetComponentInParent<Hitable>();


        if (hitable == null ||
            hitObjects.Contains(hitable) ||
            hitObjectsToIgnore.Contains(hitable))
            return;

        hitObjects.Add(hitable);

        direction = rb.linearVelocity;
        Debug.Log($"Hit: {hitable.name}, EntityStatus: {hitable.GetComponent<EntityStatus>()}");
        hitable.TakeDamage(direction, currentDamage, currentKnockback);
        OnHit?.Invoke();
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
