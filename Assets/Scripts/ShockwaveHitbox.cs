
using System.Collections;
using UnityEngine;

public class ShockwaveHitbox : Hitbox
{
    [Header("Wall Detection")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Expansion")]
    [SerializeField] private float expansionDuration = 1f;
    [SerializeField] private float maxRadius = 5f;

    private Coroutine expansionCoroutine;

    protected override void Awake()
    {
        base.Awake();
        transform.localScale = Vector3.zero;
    }

    public override void BeginAttack(Vector2 direction, DamageData[] damage, float knockback)
    {
        base.BeginAttack(direction, damage, knockback);

        if (expansionCoroutine != null)
            StopCoroutine(expansionCoroutine);

        expansionCoroutine = StartCoroutine(Expand());
    }

    private IEnumerator Expand()
    {
        float elapsed = 0f;

        transform.localScale = Vector3.zero;

        while (elapsed < expansionDuration)
        {
            float t = elapsed / expansionDuration;
            float diameter = maxRadius * 2f * t;

            transform.localScale = new Vector3(diameter, diameter, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(maxRadius * 2f, maxRadius * 2f, 1f);

        EndAttack();
        expansionCoroutine = null;
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

        RaycastHit2D hit = Physics2D.Linecast(
            attackOrigin.position,
            hitable.transform.position,
            obstacleLayer
        );

        if (hit.collider != null)
            return;

        base.OnTriggerStay2D(other);
    }
}
