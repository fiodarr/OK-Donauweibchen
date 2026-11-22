using UnityEngine;

public class PickUpFishingRod: InteractiveBase
{
    public override void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerCombat player)) return;
        
        player.PickUpFishingRod();
        Destroy(gameObject);
    }

    public override string GetInteractionPrompt()
    {
        return "Pick Up";
    }
}
