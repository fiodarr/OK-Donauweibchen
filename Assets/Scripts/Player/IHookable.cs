using UnityEngine;

public interface IHookable
{
    public void Hook(Vector2 origin, Vector2 direction, float offset, float hookTimer);
    public Vector2 GetPosition();
}
