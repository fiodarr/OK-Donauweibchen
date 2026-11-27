using UnityEngine;
public class CrabHookPoint: MonoBehaviour, IHookable
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private Crab crab;
    
    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (crab.IsPointBehind(origin))
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
        return crab.IsPointBehind(crab.GetTarget().position);
        //return true;
    }
}
