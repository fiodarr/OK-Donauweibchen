using UnityEngine;

public class PickUpLantern: MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerComponents player)) return;
        player.PickUpLantern();
        Destroy(gameObject);
    }

    public string GetInteractionPrompt()
    {
        return "Pick Up";
}
}