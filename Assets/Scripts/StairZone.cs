using UnityEngine;

public class StairZone : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;

    [Range(0.1f, 2f)]
    public float topScale = 1f;

    [Range(0.1f, 2f)]
    public float bottomScale = 0.7f;
}