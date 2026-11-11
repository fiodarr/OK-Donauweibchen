
using UnityEngine;

public class CharacterSorting : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Transform feetPoint;

    public string overLayer = "AboveWalls";
    public string behindLayer = "BehindWalls";

    public int precision = 100;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (feetPoint == null)
            feetPoint = transform;
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder =
            -Mathf.RoundToInt(feetPoint.position.y * precision);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            spriteRenderer.sortingLayerName = behindLayer;

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            spriteRenderer.sortingLayerName = overLayer;
        }
    }
}
