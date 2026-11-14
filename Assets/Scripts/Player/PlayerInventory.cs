
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int pearlsCount;

    [SerializeField] private float attractionRadius = 4f;
    [SerializeField] private float scanInterval = 0.1f;

    private readonly List<Collider2D> nearbyColliders = new();
    private ContactFilter2D pearlFilter;
    private float scanTimer;

    private void Awake()
    {
        pearlFilter = new ContactFilter2D();
        pearlFilter.SetLayerMask(Layers.Pickup);
        pearlFilter.useTriggers = true;
    }

    private void Update()
    {
        scanTimer += Time.deltaTime;

        if (scanTimer < scanInterval)
            return;

        scanTimer = 0f;
        DetectPearls();
    }

    private void DetectPearls()
    {
        nearbyColliders.Clear();

        Physics2D.OverlapCircle(
            transform.position,
            attractionRadius,
            pearlFilter,
            nearbyColliders
        );

        foreach (Collider2D col in nearbyColliders)
        {
            BasePearl pearl = col.GetComponentInParent<BasePearl>();

            if (pearl != null)
                pearl.Attract(transform, this);
        }
    }

    public int GetPearls()
    {
        return pearlsCount;
    }

    public void SetPearls(int count)
    {
        pearlsCount = Mathf.Max(0, count);
    }

    public void ChangePearls(int change)
    {
        pearlsCount = Mathf.Max(0, pearlsCount + change);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}
