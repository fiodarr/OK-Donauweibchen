using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSorting : MonoBehaviour
{
    public SortingGroup sortingGroup;
    public Transform feetPoint;

    public string overLayer = "AboveWalls";
    public string behindLayer = "BehindWalls";

    public int precision = 100;

    [Header("Stairs")]
    [SerializeField] private Transform visualRoot;

    private StairZone currentStair;
    private Vector3 originalScale;
    private float currentScale = 1f;
    private float xSign = 1f;

    private void Awake()
    {
        if (sortingGroup == null)
            sortingGroup = GetComponent<SortingGroup>();

        if (feetPoint == null)
            feetPoint = transform;

        if (visualRoot == null)
            visualRoot = transform;

        originalScale = visualRoot.localScale;
        xSign = Mathf.Sign(originalScale.x);
    }

    private void LateUpdate()
    {
        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder =
                -Mathf.RoundToInt(feetPoint.position.y * precision);
        }

        UpdateScale();
    }

    public void SetFacingDirection(float direction)
    {
        if (Mathf.Abs(direction) > 0.01f)
            xSign = Mathf.Sign(direction);
    }

    private void UpdateScale()
    {
        if (currentStair != null &&
            currentStair.topPoint != null &&
            currentStair.bottomPoint != null)
        {
            Vector2 top = currentStair.topPoint.position;
            Vector2 bottom = currentStair.bottomPoint.position;
            Vector2 direction = bottom - top;

            float t = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                t = Mathf.Clamp01(
                    Vector2.Dot(
                        (Vector2)feetPoint.position - top,
                        direction.normalized
                    ) / direction.magnitude
                );
            }

            currentScale = Mathf.Lerp(
                currentStair.topScale,
                currentStair.bottomScale,
                t
            );
        }

        visualRoot.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * currentScale * xSign,
            originalScale.y * currentScale,
            originalScale.z * currentScale
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
            return;
        if (other.TryGetComponent(out StairZone stair))
        {
            currentStair = stair;
            return;
        }

        if (other.CompareTag("Wall"))
            SetSortingLayer(behindLayer);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger)
            return;
        if (other.TryGetComponent(out StairZone stair))
        {
            if (currentStair == stair)
                currentStair = null;

            return;
        }

        if (other.CompareTag("Wall"))
            SetSortingLayer(overLayer);
    }

    private void SetSortingLayer(string layerName)
    {
        if (sortingGroup != null)
            sortingGroup.sortingLayerName = layerName;
    }
}