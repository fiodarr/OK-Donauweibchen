using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class InteractiveBase: MonoBehaviour, IInteractable
{
    [SerializeField] private InputPromptVisual interactPrompt;

    public void ChangePromptTransparency(float distance, float detectionRange, float interactionRange)
    {
        Debug.Log("Transparency changed in interactive base to");
        interactPrompt.SetAlpha(distance, detectionRange, interactionRange);
    }

    public void ShowPrompt()
    {
        interactPrompt.Show();
    }
    
    public void HidePrompt()
    {
        interactPrompt.Hide();
    }
    
    public abstract void Interact(GameObject interactor);

    public abstract string GetInteractionPrompt();
}