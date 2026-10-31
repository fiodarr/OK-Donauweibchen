using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;

    [Header("Combo")]
    public float attackCooldown = 0.5f;

    public AttackData[] attacks;

    public AttackData GetAttack(int index)
    {
        if (attacks == null || index < 0 || index >= attacks.Length)
            return null;

        return attacks[index];
    }
}