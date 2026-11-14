using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargeChooseUtility: MonoBehaviour
{
    private HashSet<IHookable> hookables =  new HashSet<IHookable>();

    public IHookable GetTarget(Transform pivot)
    {
        return GetClosestTarget(pivot);
    }
    
    public IHookable GetClosestTarget(Transform pivot)
    {
        if (hookables.Count == 0)
        {
            return null;
        }
        IHookable closest = hookables.First();
        float closestDistance = float.MaxValue;
        foreach (IHookable h in hookables)
        {
            float distance = Vector2.Distance(pivot.position, h.GetPosition());
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = h;
            }
        }
        return closest;
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Remove(h);
            //Debug.Log(collision.name + " has exited hook range for " + name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Add(h);
            //Debug.Log(collision.name + " has exited hook range for " + name);
        }
    }
}
