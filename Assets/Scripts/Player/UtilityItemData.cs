using System;
using UnityEngine;

[Serializable]
public class UtilityItemData
{
    public enum TargetPriority
    {
        Default
    }
    
    public TargetPriority priority;
    public Collider2D lookupTriangleArea;
}
