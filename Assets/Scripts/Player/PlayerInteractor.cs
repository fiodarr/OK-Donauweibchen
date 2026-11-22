using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerComponents playerComponents;
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private float detectionRadius = 3.5f;

    private InteractiveBase detectedInteractable;

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
            detectionRadius,
            Layers.Interactables | Layers.RestSpots
        );
        
        if (hit != null)
        {
            InteractiveBase interactiveBase = hit.GetComponent<InteractiveBase>();

            if (interactiveBase == null)
            {
                if (detectedInteractable != null)
                {
                    detectedInteractable.HidePrompt();
                    detectedInteractable = null;
                }
                return;
            }
            
            if (interactiveBase != detectedInteractable)
            {
                if (detectedInteractable != null)
                {
                    detectedInteractable.HidePrompt();
                }
                detectedInteractable = interactiveBase;
                detectedInteractable.ShowPrompt();
            }
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            detectedInteractable.ChangePromptTransparency(distance, detectionRadius, interactionRadius);
        }
    }

    private void Interact()
    {
        if (detectedInteractable == null) return;
        
        if (Vector2.Distance(detectedInteractable.transform.position, transform.position) < interactionRadius) 
            detectedInteractable?.Interact(this.gameObject);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}