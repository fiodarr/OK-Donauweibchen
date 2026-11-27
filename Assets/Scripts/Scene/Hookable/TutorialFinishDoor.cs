using UnityEngine;

public class TutorialFinishDoor: MonoBehaviour, IHookable
{
    [SerializeField] private FinishDoor doorObject;

    [SerializeField] private GameObject closed,open;
    private bool isUnlocked = false;

    public void Hook(Vector2 origin, Vector2 direction, float offset, float pullSpeed)
    {
        Debug.Log("TryUnlock");
        if (isUnlocked) return;
        doorObject.Disappear();
        closed.SetActive(false);
        open.SetActive(true);
    }

    public Vector2 GetPosition()
    {
        return transform.position;
    }

    public bool AllowedToHook()
    {
        return !isUnlocked;
    }
}
