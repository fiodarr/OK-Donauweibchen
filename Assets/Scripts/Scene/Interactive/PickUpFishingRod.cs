using UnityEngine;

public class PickUpFishingRod: MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerCombat player)) return;
        
        player.PickUpFishingRod();
        Destroy(gameObject);
    }

    public string GetInteractionPrompt()
    {
        return "Pick Up";
    }
}
