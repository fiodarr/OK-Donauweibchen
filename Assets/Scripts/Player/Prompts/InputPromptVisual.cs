using UnityEngine;
using UnityEngine.UI;

public class InputPromptVisual: MonoBehaviour
{
    [SerializeField] private InputVisual sprites;
    [SerializeField] private GameObject visuals;
    [SerializeField] private Image image;
    
    public void Show()
    {
        ChangeSprite();
        visuals.SetActive(true);
    }

    public void Hide()
    {
        visuals.SetActive(false);
    }
    
    public void UpdateSprite()
    {
        ChangeSprite();
    }

    public void SetAlpha(float distance, float detectionRange, float interactionRange)
    {
        float alpha = 1 - (distance - interactionRange) / (detectionRange - interactionRange);
        //Debug.Log("Transparency changed in input prompt");
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
    
    private void ChangeSprite()
    {
        switch (GameManager.Instance.currentControlScheme)
        {
            case PlayerInputHandler.ControlScheme.KeyboardMouse:
                image.sprite = sprites.keyboardSchemeSprite;
                Debug.Log("Now using keyboard sprite");
                break;
            case PlayerInputHandler.ControlScheme.Controller:
                image.sprite = sprites.playstationSchemeSprite;
                Debug.Log("Now using playstation sprite");
                break;
            case PlayerInputHandler.ControlScheme.XboxController:
                image.sprite =  sprites.xboxSchemeSprite;
                Debug.Log("Now using xbox sprite");
                break;
            default:
                Debug.LogError($"Unknown control scheme {GameManager.Instance.currentControlScheme}");
                break;
        }
    }
}
