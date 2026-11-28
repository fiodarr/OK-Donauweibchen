using UnityEngine;
using UnityEngine.UI;

public class HealthChunk: MonoBehaviour
{
    [SerializeField] private Color armoredColor;
    [SerializeField] private Color nonArmoredColor;
    [SerializeField] private Image healthBarFill;
    
    public bool isArmored = false;
    private float maxValue = 0;
    private float currentValue = 0;

    public void SetMaxValue(float maxValue)
    {
        this.maxValue = maxValue;
        currentValue = maxValue;

        if (isArmored)
        {
            healthBarFill.color = armoredColor;
        }
        else
        {
            healthBarFill.color = nonArmoredColor;
        }
        
        healthBarFill.fillAmount = 1f;
    }

    public void SetCurrentValue(float currentValue)
    {
        this.currentValue = Mathf.Clamp(currentValue, 0, maxValue);

        healthBarFill.fillAmount = maxValue > 0
            ? this.currentValue / maxValue
            : 0f;
    }

    public void RemoveArmor()
    {
        if (!isArmored)
            return;
        
        isArmored = false;
        healthBarFill.color = nonArmoredColor;
        SetCurrentValue(currentValue);
    }

    public float GetMaxValue()
    {
        return maxValue;
    }

    
}
