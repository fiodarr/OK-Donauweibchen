using UnityEngine;

[System.Serializable]
public class AttackData
{
    [Header("Attack")]
    public DamageData[] damage;
    public float knockback = 5f;
    public float launchForce = 5f;
    
    [Header("Sound")]
    public SoundEvent attackSound;

    [Header("Timing")]
    public float activeTime = 0.15f;
    public float recoveryTime = 0.25f;
    public float comboTime = 0.5f;

    [Header("Hitbox")]
    public Hitbox hitbox;
}