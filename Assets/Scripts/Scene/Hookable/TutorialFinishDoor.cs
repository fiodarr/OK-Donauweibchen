using UnityEngine;

public class TutorialFinishDoor: MonoBehaviour, IHookable
{
    [SerializeField] private FinishDoor doorObject;
    private bool isUnlocked = false;

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        if (isUnlocked) return;
        doorObject.Disappear();
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
