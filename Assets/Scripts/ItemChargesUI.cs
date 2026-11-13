
using UnityEngine;
using UnityEngine.UI;

public class ItemChargesUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform grid;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Lantern lantern;

    [Header("Colors")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color consumedColor = Color.gray;

    private Image[] chargeDots;
    private int currentMaxCharges = -1;

    private void OnEnable()
    {
        lantern.OnChargesChanged += UpdateCharges;
    }

    private void OnDisable()
    {
        lantern.OnChargesChanged -= UpdateCharges;
    }

    private void Start()
    {
        UpdateCharges(lantern.CurrentCharges, lantern.MaxCharges);
    }

    private void UpdateCharges(int currentCharges, int maxCharges)
    {
        maxCharges = Mathf.Max(0, maxCharges);
        currentCharges = Mathf.Clamp(currentCharges, 0, maxCharges);

        if (currentMaxCharges != maxCharges)
        {
            RebuildDots(maxCharges);
        }

        for (int i = 0; i < chargeDots.Length; i++)
        {
            chargeDots[i].color =
                i < currentCharges ? availableColor : consumedColor;
        }
    }

    private void RebuildDots(int maxCharges)
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        chargeDots = new Image[maxCharges];

        for (int i = 0; i < maxCharges; i++)
        {
            GameObject dot = Instantiate(dotPrefab, grid);
            chargeDots[i] = dot.transform.GetChild(0).GetComponent<Image>();
        }

        currentMaxCharges = maxCharges;
    }
}
