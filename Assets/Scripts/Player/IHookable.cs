using UnityEngine;

public interface IHookable
{
    public void Hook(Vector2 origin, Vector2 direction, float offset);
    public Vector2 GetPosition();
}
