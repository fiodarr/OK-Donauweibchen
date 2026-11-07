using UnityEngine;
using UnityEngine.UI;

public class HealthbarTest : MonoBehaviour
{
    [SerializeField] private EntityStatus status;
    [SerializeField] private Image image;

    private void Start()
    {
        status.OnHealthChanged+=UpdateBar;
    }
    private void UpdateBar(float amount, float max)
    {
        image.fillAmount = amount/max;
    }
}
