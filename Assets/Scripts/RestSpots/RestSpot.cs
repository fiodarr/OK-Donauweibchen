using UnityEngine;

public class RestSpot : InteractiveBase
{
    public string restSpotID;
    public bool isActivated;

    public void Activate()
    {
        if (isActivated) return;

        isActivated = true;

        Debug.Log("Rest spot activated: " + restSpotID);
    }

    public override void Interact(GameObject interactor)
    {
        RestSpotManager.Instance.RestAtRestSpot(this);
    }

    public override string GetInteractionPrompt()
    {
        return "Rest";
    }
}