using UnityEngine;

public enum DamageType
{
    Physical,
    Armor
}

[System.Serializable]
public class DamageData
{
    public float amount = 5f;
    public DamageType type = DamageType.Physical;
}