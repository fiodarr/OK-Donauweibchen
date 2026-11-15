using UnityEngine;

public class Planks: MonoBehaviour, IHookable
{
    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        Destroy(this.gameObject);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
