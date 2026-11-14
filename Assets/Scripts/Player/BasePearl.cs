using System.Collections;
using UnityEngine;

public class BasePearl : MonoBehaviour
{
    [SerializeField] private int value = 1;

    [SerializeField] private float attractionSpeed = 8f;
    [SerializeField] private float collectionDistance = 0.2f;

    private Transform target;
    private PlayerInventory inventory;

    private bool isAttracted;
    private bool isCollected;

    [SerializeField] private AnimationCurve attractionSpeedCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    public void Attract(Transform player, PlayerInventory playerInventory)
    {
        if (isAttracted || isCollected || player == null || playerInventory == null)
            return;

        target = player;
        inventory = playerInventory;
        isAttracted = true;

        if (TryGetComponent(out Collider2D pearlCollider))
            pearlCollider.enabled = false;

        StartCoroutine(AttractionRoutine());
    }

    private IEnumerator AttractionRoutine()
    {
        float startingDistance = Vector2.Distance(transform.position, target.position);
        Vector3 startingScale = transform.localScale;
        while (target != null)
        {
            float currentDistance = Vector2.Distance(transform.position, target.position);

            float normalizedDistance = 1f - Mathf.InverseLerp(
                0f,
                startingDistance,
                currentDistance
            );

            // Movement speed based on curve
            float currentSpeed = attractionSpeed * attractionSpeedCurve.Evaluate(normalizedDistance);

            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                currentSpeed * Time.deltaTime
            );

            // Local scale based on curve
            float scale = scaleCurve.Evaluate(normalizedDistance);
            transform.localScale = startingScale * scale;

            if (Vector2.Distance(transform.position, target.position) <= collectionDistance)
            {
                Collect();
                yield break;
            }

            yield return null;
        }
    }

    private void Collect()
    {
        if (isCollected)
            return;

        isCollected = true;

        inventory.ChangePearls(value);
        Destroy(gameObject);
    }
}