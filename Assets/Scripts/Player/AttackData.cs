using FMODUnity;
using UnityEngine;

[System.Serializable]
public class AttackData
{
    [Header("Attack")]
    public DamageData[] damage;
    public float knockback = 5f;
    public float launchForce = 5f;
    
    [Header("Sound")]
    public EventReference attackSound;
    public EventReference chargeSound;

    [Header("Vibration")]

    public float vibrationVelocity;
    public float vibrationLength;

    [Header("Timing")]
    public float chargeTime = 0f;
    public float activeTime = 0.15f;
    public float recoveryTime = 0.25f;
    public float comboTime = 0.5f;

    [Header("Hitbox")]
    public Hitbox hitbox;
}