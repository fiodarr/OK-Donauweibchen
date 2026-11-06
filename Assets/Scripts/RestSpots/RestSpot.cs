using UnityEngine;

public class RestSpot : MonoBehaviour, IInteractable
{
    public string restSpotID;
    public bool isActivated;

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;

        Debug.Log("Rest spot activated: " + restSpotID);
    }

    public void Interact()
    {
        RestSpotManager.Instance.RestAtRestSpot(this);
    }

    public string GetInteractionPrompt()
    {
        return "Rest";
    }
}