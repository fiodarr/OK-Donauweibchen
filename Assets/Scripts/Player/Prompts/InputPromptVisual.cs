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

    private void Start()
    {
        GameManager.Instance.controlSchemeChanged += UpdateSprite;
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
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }
    
    private void ChangeSprite()
    {
        switch (GameManager.Instance.CurrentControlScheme)
        {
            case PlayerInputHandler.ControlScheme.KeyboardMouse:
                image.sprite = sprites.keyboardSchemeSprite;
                break;
            case PlayerInputHandler.ControlScheme.Controller:
                image.sprite = sprites.playstationSchemeSprite;
                break;
            case PlayerInputHandler.ControlScheme.XboxController:
                image.sprite =  sprites.xboxSchemeSprite;
                break;
            default:
                Debug.LogError($"Unknown control scheme {GameManager.Instance.CurrentControlScheme}");
                break;
        }
    }
    
    
}
