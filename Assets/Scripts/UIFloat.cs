using UnityEngine;

public class UIFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float frequency = 1f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.unscaledTime * frequency) * amplitude;
        rectTransform.anchoredPosition = startPosition + Vector2.up * offset;
    }
}