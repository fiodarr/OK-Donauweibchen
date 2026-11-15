using UnityEngine;

public class JumpOffObject: MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject destinationPoint;
    [SerializeField] private float jumpDuration = 0.7f;

    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController player))

        if (player == null || destinationPoint == null)
            return;

        player.TryStartCliffJump(
            destinationPoint.transform.position,
            jumpDuration
        );
    }

    public string GetInteractionPrompt()
    {
        return "Jump Off";
    }
}