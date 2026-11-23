using System.Collections.Generic;
using UnityEngine;

public class TargetChooseUtility : MonoBehaviour
{
    private HashSet<IHookable> hookables = new HashSet<IHookable>();

    private int wallLayerMask;

    private void Awake()
    {
        wallLayerMask = LayerMask.GetMask("Walls");
    }

    public IHookable GetTarget(Transform pivot)
    {
        return GetClosestTarget(pivot);
    }

    public IHookable GetClosestTarget(Transform pivot)
    {
        if (hookables.Count == 0)
            return null;

        IHookable closest = null;
        float closestDistance = float.MaxValue;

        foreach (IHookable h in hookables)
        {
            if (h == null)
                continue;

            Vector2 targetPosition = h.GetPosition();

            float distance = Vector2.Distance(
                pivot.position,
                targetPosition
            );

            if (distance < closestDistance &&
                CanHookTarget(pivot, targetPosition))
            {
                closestDistance = distance;
                closest = h;
            }
        }

        return closest;
    }

    private bool CanHookTarget(
        Transform pivot,
        Vector2 targetPosition)
    {
        Vector2 direction =
            targetPosition - (Vector2)pivot.position;

        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return true;

        RaycastHit2D hit = Physics2D.Raycast(
            pivot.position,
            direction.normalized,
            distance,
            wallLayerMask
        );

        return hit.collider == null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Add(h);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Remove(h);
        }
    }
}