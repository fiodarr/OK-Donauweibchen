using UnityEngine;

public class PickUpFishingRod: MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerCombat player)) return;
        
        player.PickUpFishingRod();
        PopupManager.Instance.TriggerPopup(PopupEvent.FishingRodPickup);
        Destroy(gameObject);
    }

    public string GetInteractionPrompt()
    {
        return "Pick Up";
    }
}
