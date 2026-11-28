using UnityEngine;
public class CrabHookPoint: MonoBehaviour, IHookable
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private Crab crab;
    public bool canHook = true;
    
    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (crab.IsPointBehind(origin) && crab.HasArmor)
        {
            crab.RemoveArmour();
        }
    }

    public Vector2 GetPosition()
    {
        return pivot.transform.position;
    }

    public bool AllowedToHook()
    {
        if (!crab.IsPointBehind(crab.GetTarget().transform.position))
        {
            Debug.Log("Not behind crab");
        }

        if (!crab.HasArmor)
        {
            Debug.Log("Doesn't have armor");
        }
        return (crab.IsPointBehind(crab.GetTarget().transform.position) && crab.HasArmor);
    }
}
