using System.Collections;
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
    private bool canClose = false;

    private IEnumerator CanCloseRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        canClose = true;
    }

    private void Start()
    {
        powerupButton.onClick.AddListener(TryBuyPowerup);
    }

    public void Show()
    {
        
        Debug.Log("HEREHERE");
        visuals.SetActive(true);
        active = true;
        feedbackText.text = "Press X (ps) / A (xbox)/ Ctrl (Keyboard) to buy";
        feedbackText.color = Color.white;
        StartCoroutine(CanCloseRoutine());
    }

    private void Hide()
    {
        if (!canClose) return;
        Debug.Log("I hide");
        visuals.SetActive(false);
        active = false;
        canClose = false;
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
        powerupButton.interactable = false;
        feedbackText.text = "Success!";
        feedbackText.color = Color.green;
        powerupText.text = "Double Dash - obtained";
        powerupText.color = Color.green;
    }
}
