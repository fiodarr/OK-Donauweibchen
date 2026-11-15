using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupBuyMenu: MonoBehaviour
{
    [SerializeField] private GameObject visuals;
    [SerializeField] private Button powerupButton;
    [SerializeField] private TextMeshProUGUI powerupText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private float price;
    
    private bool active = false;

    private void Start()
    {
        powerupButton.onClick.AddListener(TryBuyPowerup);
    }

    public void Show()
    {
        inputHandler.MenuConfirm += TryBuyPowerup;
        inputHandler.MenuClose += Hide;
        visuals.SetActive(true);
        active = true;
        feedbackText.text = "Press X (ps) / A (xbox)/ Ctrl (Keyboard) to buy";
        feedbackText.color = Color.white;
    }

    private void Hide()
    {
        visuals.SetActive(false);
        active = false;
        inputHandler.MenuConfirm -= TryBuyPowerup;
        inputHandler.MenuClose -= Hide;
    }
    

    private void TryBuyPowerup()
    {
        if (playerInventory.GetPearls() < price)
        {
            Debug.Log("You do not have enough money!");
            feedbackText.text = "Not enough pearls!";
            feedbackText.color = Color.red;
            return;
        }
        Debug.Log("Power up!");
        playerInventory.ChangePearls((int) -price);
        playerUpgrades.isUpgraded = true;
        powerupButton.interactable = false;
        feedbackText.text = "Success!";
        feedbackText.color = Color.green;
        powerupText.text = "Double Dash - obtained";
        powerupText.color = Color.green;
    }
}
