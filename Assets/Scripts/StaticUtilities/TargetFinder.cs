using UnityEngine;

public static class TargetFinder
{
    public static T FindClosestInCircleByDistance<T>(Vector2 origin,
        float radius,
        Vector2 forwardDirection,
        LayerMask layerMask,
        bool isHalfCircle) where T : Component
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius, layerMask);
        
        T closestComponent = null;
        float minSqrDistance = float.MaxValue; 

        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent(out T component))
            {
                Vector2 vectorToTarget = (Vector2)col.transform.position - origin;
                
                if (isHalfCircle && Vector2.Dot(forwardDirection.normalized, vectorToTarget.normalized) < 0f)
                {
                    continue; 
                }

                float sqrDist = vectorToTarget.sqrMagnitude;
                
                if (sqrDist < minSqrDistance)
                {
                    minSqrDistance = sqrDist;
                    closestComponent = component;
                }
            }
        }
        
        return closestComponent;
    }
    public static T FindClosestInCircleByAngle<T>(
        Vector2 origin, 
        float radius, 
        Vector2 forwardDir, 
        LayerMask layerMask, 
        bool isHalfCircle) where T : Component
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius, layerMask);

        if (colliders.Length == 0) return null;
        
        T closestComponent = null;
        float minAngle = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent(out T component))
            {
                Vector2 vectorToTarget = (Vector2)col.transform.position - origin;
                
                float currentAngle = Vector2.Angle(forwardDir, vectorToTarget);
                if (isHalfCircle && currentAngle > 90f)
                {
                    continue;
                }

                if (currentAngle < minAngle)
                {
                    minAngle = currentAngle;
                    closestComponent = component;
                }
            }
        }
        
        return closestComponent;
    }
    
    public static Vector2 GetDirectionToClosestByDistance<T>(
        Vector2 origin, 
        float radius, 
        Vector2 forwardDir, 
        LayerMask layerMask, 
        bool isHalfCircle) where T : Component
    {
        T closest = FindClosestInCircleByDistance<T>(origin, radius, forwardDir, layerMask, isHalfCircle);
        
        if (closest == null)
        {
            return Vector2.zero;
        }

        return ((Vector2)closest.transform.position - origin).normalized;
    }
    
    public static Vector2 GetDirectionToClosestByAngle<T>(
        Vector2 origin, 
        float radius, 
        Vector2 forwardDir, 
        LayerMask layerMask, 
        bool isHalfCircle) where T : Component
    {
        T closest = FindClosestInCircleByAngle<T>(origin, radius, forwardDir, layerMask, isHalfCircle);
        
        if (closest == null)
        {
            return Vector2.zero;
        }

        return ((Vector2)closest.transform.position - origin).normalized;
    }
}
