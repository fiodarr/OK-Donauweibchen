using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerComponents playerComponents;
    [SerializeField] private float interactionRadius = 1.5f;

    private IInteractable currentInteractable;

    private void OnEnable()
    {
        playerComponents.inputHandler.InteractPressed += Interact;
    }

    private void OnDisable()
    {
        playerComponents.inputHandler.InteractPressed -= Interact;
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            interactionRadius,
            Layers.Interactables | Layers.RestSpots
        );

        currentInteractable = null;

        if (hit != null)
        {
            currentInteractable = hit.GetComponent<IInteractable>();
        }
    }

    private void Interact()
    {
        currentInteractable?.Interact(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}