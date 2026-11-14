using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private TMP_Text currencyText;

    private void OnEnable()
    {
        inventory.OnPearlsChanged += UpdateCurrency;
        UpdateCurrency(inventory.GetPearls());
    }

    private void OnDisable()
    {
        inventory.OnPearlsChanged -= UpdateCurrency;
    }

    private void UpdateCurrency(int amount)
    {
        currencyText.text = amount.ToString();
    }
}