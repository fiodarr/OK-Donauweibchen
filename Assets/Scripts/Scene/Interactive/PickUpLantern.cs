using UnityEngine;

public class PickUpLantern: InteractiveBase
{
    public override void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerComponents player)) return;
        player.PickUpLantern();
        Destroy(gameObject);
    }

    public override string GetInteractionPrompt()
    {
        return "Pick Up";
    }
}