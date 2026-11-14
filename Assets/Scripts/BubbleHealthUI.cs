
using UnityEngine;
using UnityEngine.UI;

public class BubbleHealthUI : MonoBehaviour
{
    [Header("Bubble Images")]
    [SerializeField] private Image[] bubbles;

    [Header("Bubble Sprites")]
    [SerializeField] private Sprite fullBubble;
    [SerializeField] private Sprite emptyBubble;

    [Header("Bubble Size")]
    [SerializeField] private float halfBubbleScale = 0.65f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private int currentHealth = 8;
    [SerializeField] private EntityStatus status;

    private void OnEnable()
    {
        status.OnHealthChanged += UpdateHealth;
    }

    private void OnDisable()
    {
        status.OnHealthChanged -= UpdateHealth;
    }

    private void Start()
    {
        UpdateHealth(currentHealth, maxHealth);
    }

    private void UpdateHealth(float health, float maximumHealth)
    {
        maxHealth = Mathf.Max(0, Mathf.RoundToInt(maximumHealth));
        currentHealth = Mathf.Clamp(Mathf.RoundToInt(health), 0, maxHealth);

        int requiredBubbles = Mathf.CeilToInt(maxHealth / 2f);

        for (int i = 0; i < bubbles.Length; i++)
        {
            if (i >= requiredBubbles)
            {
                bubbles[i].enabled = false;
                bubbles[i].rectTransform.localScale = Vector3.one;
                continue;
            }

            bubbles[i].enabled = true;

            int bubbleCapacity = Mathf.Min(2, maxHealth - i * 2);
            int bubbleHealth = Mathf.Clamp(
                currentHealth - i * 2,
                0,
                bubbleCapacity
            );

            if (bubbleHealth == bubbleCapacity)
            {
                bubbles[i].sprite = fullBubble;
                bubbles[i].rectTransform.localScale = Vector3.one;
            }
            else if (bubbleHealth > 0)
            {
                bubbles[i].sprite = fullBubble;
                bubbles[i].rectTransform.localScale =
                    Vector3.one * halfBubbleScale;
            }
            else
            {
                bubbles[i].sprite = emptyBubble;
                bubbles[i].rectTransform.localScale = Vector3.one;
            }
        }
    }
}
