using UnityEngine;

public static class Layers
{
    public static readonly int Player = LayerMask.GetMask("Player");
    public static readonly int Hitboxes = LayerMask.GetMask("Hitboxes");
    public static readonly int Walls = LayerMask.GetMask("Walls");
    public static readonly int Hookable = LayerMask.GetMask("Hookable");

    public static readonly int IgnoreHits = LayerMask.GetMask("IgnoreHits");
}