using UnityEngine;

public class JumpOffObject: MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject destinationPoint;
    [SerializeField] private float jumpDuration = 0.7f;

    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    
    public void Interact(GameObject interactor)
    {
        if (!interactor.TryGetComponent(out PlayerController player))

        if (player == null || destinationPoint == null)
            return;

        player.TryStartCliffJump(
            destinationPoint.transform.position,
            jumpDuration,
            movementCurve,
            heightCurve
        );
    }

    public string GetInteractionPrompt()
    {
        return "Jump Off";
    }
}