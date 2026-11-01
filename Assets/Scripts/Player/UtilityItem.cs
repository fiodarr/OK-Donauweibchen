using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UtilityItem: MonoBehaviour
{
    [SerializeField] private Collider2D lookupTriangle;
    [SerializeField] private float afterHookDistance;
    private float lookupRange;

    private HashSet<IHookable> hookables = new HashSet<IHookable>();
    
    private void Start()
    {
        lookupRange = lookupTriangle.bounds.size.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Add(h);
            Debug.Log(collision.name + " has entered hook range");
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHookable h))
        {
            hookables.Remove(h);
            Debug.Log(collision.name + " has exited hook range");
        }
    }

    public void UpdateLookupDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void HookDefault()
    {
        IHookable closest = FindClosestTarget();
       
        if (closest != null)
        {
            Vector2 direction =  closest.GetPosition() - (Vector2) this.transform.position;
            closest.Hook(this.transform.position,  direction, afterHookDistance);
        }
    }

    public void HookSmart(Vector2 direction)
    {
        IHookable h = SmartLookupTarget(direction);
        
        if (h != null)
        {
            h.Hook(this.transform.position,  direction, afterHookDistance);
        }
        else
        {
            HookDefault();
        }
    }
    
    private IHookable FindClosestTarget()
    {
        if (hookables.Count == 0)
        {
            return null;
        }
        IHookable closest = hookables.First();
        float closestDistance = float.MaxValue;
        foreach (IHookable h in hookables)
        {
            float distance = Vector2.Distance(h.GetPosition(), this.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = h;
            }
        }
        return closest;
    }

    private IHookable SmartLookupTarget(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            this.transform.position, 
            direction, 
            lookupRange,
            Layers.Hookable
        );

        if (!hit)
        {
            return null;
        }
        
        hit.collider.TryGetComponent(out IHookable h);
        return h;
    }
    
    
}
