
using UnityEngine;
using UnityEngine.Rendering;

public class CharacterSorting : MonoBehaviour
{
    public SortingGroup sortingGroup;
    public Transform feetPoint;

    public string overLayer = "AboveWalls";
    public string behindLayer = "BehindWalls";

    public int precision = 100;

    private void Awake()
    {
        if (sortingGroup == null)
            sortingGroup = GetComponent<SortingGroup>();

        if (feetPoint == null)
            feetPoint = transform;
    }

    private void LateUpdate()
    {
        if (sortingGroup == null)
            return;

        sortingGroup.sortingOrder =
            -Mathf.RoundToInt(feetPoint.position.y * precision);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            SetSortingLayer(behindLayer);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            SetSortingLayer(overLayer);
        }
    }

    private void SetSortingLayer(string layerName)
    {
        if (sortingGroup != null)
            sortingGroup.sortingLayerName = layerName;
    }
}
