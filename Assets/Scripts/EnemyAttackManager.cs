using System.Collections.Generic;
using UnityEngine;

public static class EnemyAttackManager
{
    private const int MaxAttackers = 2;

    private static HashSet<BaseEnemy> attackers = new();

    public static bool TryAcquire(BaseEnemy enemy)
    {
        if (attackers.Contains(enemy))
            return true;

        if (attackers.Count >= MaxAttackers)
            return false;

        attackers.Add(enemy);
        return true;
    }

    public static void Release(BaseEnemy enemy)
    {
        attackers.Remove(enemy);
    }
}